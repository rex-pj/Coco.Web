import * as React from "react";
import { useState, useRef, useEffect, useContext } from "react";
import styled from "styled-components";
import { useLocation, useNavigate } from "react-router-dom";
import { PanelHeading, PanelDefault, PanelBody } from "../../molecules/Panels";
import ImageThumb from "../../molecules/Images/ImageThumb";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import AuthorProfile from "../ProfileCard/AuthorProfile";
import { ActionButton } from "../../molecules/ButtonGroups";
import { secondaryTitleLink } from "../../atoms/Titles/TitleLinks";
import { HorizontalList } from "../../molecules/List";
import { FontButtonItem } from "../../molecules/ActionIcons";
import { AnchorLink } from "../../atoms/Links";
import { HorizontalReactBar } from "../../molecules/Reaction";
import Overlay from "../../atoms/Overlay";
import { PriceLabel } from "../../molecules/PriceAndCurrency";
import ContentItemDropdown from "../../molecules/DropdownButton/ContentItemDropdown";
import ModuleMenuListItem from "../../molecules/MenuList/ModuleMenuListItem";
import { SessionContext } from "../../../store/context/session-context";
import DeleteConfirmationModal from "../Modals/DeleteConfirmationModal";

const Panel = styled(PanelDefault)`
  position: relative;
  margin-bottom: ${(p) => p.theme.size.distance};

  .no-image {
    height: 140px;
  }
`;

const ContentBody = styled.div`
  padding: 0 0 ${(p) => p.theme.size.distance} 0;
  height: 160px;
`;

const ContentTopbar = styled.div`
  margin-bottom: ${(p) => p.theme.size.exSmall};
  position: relative;

  ${ModuleMenuListItem} {
    margin-top: 0;
    margin-bottom: 0;
    border-bottom: 1px solid ${(p) => p.theme.color.neutralBg};
  }

  ${ModuleMenuListItem}:last-child {
    border-bottom: 0;
  }

  ${ModuleMenuListItem} span {
    padding-top: ${(p) => p.theme.size.tiny};
    padding-bottom: ${(p) => p.theme.size.tiny};
  }
`;

const PostActions = styled.div`
  text-align: right;
`;

const PostTitle = styled(secondaryTitleLink)`
  margin-bottom: 0;
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
`;

const InteractiveItem = styled.li`
  margin-right: ${(p) => p.theme.size.small};
  :last-child {
    margin-right: 0;
  }
`;

const ThumbnailBox = styled.div`
  position: relative;

  img {
    border-top-left-radius: ${(p) => p.theme.borderRadius.normal};
    border-top-right-radius: ${(p) => p.theme.borderRadius.normal};
  }
`;

const PanelHeader = styled(PanelHeading)`
  padding-bottom: 0;
`;

const InteractItem = styled(InteractiveItem)`
  margin-right: 0;
  vertical-align: middle;
`;

const InteractRightItem = styled(InteractiveItem)`
  text-align: right;
  float: right;

  .add-to-cart {
    border-radius: ${(p) => p.theme.borderRadius.normal};
    padding: ${(p) => p.theme.size.exTiny};
    margin-top: -${(p) => p.theme.size.exTiny};
  }

  .add-to-cart:hover {
    background-color: ${(p) => p.theme.color.secondaryBg};
    color: ${(p) => p.theme.color.neutralText};
  }
`;

const RowItem = styled.div`
  margin-bottom: ${(p) => p.theme.size.exTiny};
`;

const FarmInfo = styled(RowItem)`
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: ${(p) => p.theme.fontSize.tiny};

  a {
    vertical-align: middle;
    font-weight: 600;
    color: ${(p) => p.theme.color.primaryText};
  }

  svg {
    margin-right: ${(p) => p.theme.size.exTiny};
    font-size: ${(p) => p.theme.fontSize.tiny};
    color: ${(p) => p.theme.color.primaryText};
    vertical-align: middle;
  }

  path {
    color: inherit;
  }
`;

interface Props {
  product?: any;
  onOpenDeleteConfirmationModal: (e: any) => void;
}

const ProductItem = (props: Props) => {
  const location = useLocation();
  const naviagte = useNavigate();
  const { product, onOpenDeleteConfirmationModal } = props;
  const { creator, createdByIdentityId } = product;
  const { currentUser, isLogin } = useContext(SessionContext);
  const isAuthor =
    currentUser && createdByIdentityId === currentUser.userIdentityId;
  const [isActionDropdownShown, setActionDropdownShown] = useState(false);
  const currentRef = useRef<any>();
  const onActionDropdownHide = (e: MouseEvent) => {
    if (currentRef.current && !currentRef.current.contains(e.target)) {
      setActionDropdownShown(false);
    }
  };

  const onActionDropdownShow = () => {
    setActionDropdownShown(true);
  };

  const onEditMode = async () => {
    naviagte(`/products/update/${product.id}`, {
      state: {
        from: location.pathname,
      },
    });
  };

  useEffect(() => {
    document.addEventListener("click", onActionDropdownHide, false);
    return () => {
      document.removeEventListener("click", onActionDropdownHide);
    };
  });

  const loadCreatedInfo = () => {
    if (creator) {
      creator.info = "Farmer";
    }
    return creator;
  };

  let description = "";
  if (product.description && product.description.length >= 120) {
    description = `${product.description.substring(0, 120)}...`;
  } else {
    description = product.description;
  }

  const onOpenDeleteConfirmation = () => {
    onOpenDeleteConfirmationModal({
      title: "Delete Farm",
      innerModal: DeleteConfirmationModal,
      message: `Are you sure to delete product "${product.name}"?`,
      id: parseFloat(product.id),
    });
  };

  return (
    <Panel>
      <ThumbnailBox>
        <ImageThumb src={product.pictureUrl} alt="" />
        <Overlay />
      </ThumbnailBox>

      <PanelHeader>
        <ContentTopbar>
          <div className="row g-0">
            <div className="col col-8 col-sm-9 col-md-10 col-lg-11">
              <AuthorProfile profile={loadCreatedInfo()} />
            </div>

            <div className="col col-4 col-sm-3 col-md-2 col-lg-1">
              {isLogin ? (
                <PostActions ref={currentRef}>
                  <ActionButton onClick={onActionDropdownShow}>
                    <FontAwesomeIcon icon="angle-down" />
                  </ActionButton>
                </PostActions>
              ) : null}
              {isActionDropdownShown && isAuthor ? (
                <ContentItemDropdown>
                  <ModuleMenuListItem>
                    <span onClick={onEditMode}>
                      <FontAwesomeIcon
                        icon="pencil-alt"
                        className="me-2"
                      ></FontAwesomeIcon>
                      Edit
                    </span>
                  </ModuleMenuListItem>
                  <ModuleMenuListItem>
                    <span onClick={onOpenDeleteConfirmation}>
                      <FontAwesomeIcon
                        icon="trash-alt"
                        className="me-2"
                      ></FontAwesomeIcon>
                      Delete
                    </span>
                  </ModuleMenuListItem>
                </ContentItemDropdown>
              ) : null}
            </div>
          </div>
        </ContentTopbar>
        <PostTitle>
          <AnchorLink
            to={{
              pathname: product.url,
              state: { from: location.pathname },
            }}
          >
            {product.name}
          </AnchorLink>
        </PostTitle>
      </PanelHeader>
      <PanelBody>
        <RowItem>
          {product.price > 0 ? (
            <PriceLabel price={product.price} currency="vnđ" />
          ) : null}
        </RowItem>
        {product.farms
          ? product.farms.map((pf: any) => {
              if (!pf.id) {
                return null;
              }

              return (
                <FarmInfo key={pf.id}>
                  <FontAwesomeIcon icon="warehouse" />
                  <AnchorLink
                    to={{
                      pathname: pf.url,
                      state: { from: location.pathname },
                    }}
                  >
                    {pf.name}
                  </AnchorLink>
                </FarmInfo>
              );
            })
          : null}

        <div className="panel-content">
          <ContentBody>
            {description ? (
              <p dangerouslySetInnerHTML={{ __html: description }}></p>
            ) : null}
          </ContentBody>
        </div>
        {/* <HorizontalList className="clearfix">
          <InteractItem>
            <HorizontalReactBar reactionNumber={100} className="me-3" />
            <FontButtonItem icon="comments" dynamicText={200} />
          </InteractItem>
          <InteractRightItem>
            <FontButtonItem className="add-to-cart" icon="shopping-bag" />
          </InteractRightItem>
        </HorizontalList> */}
      </PanelBody>
    </Panel>
  );
};

export default ProductItem;
