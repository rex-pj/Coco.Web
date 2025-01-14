import * as React from "react";
import { Fragment, useState, useRef, useEffect, useContext } from "react";
import AuthorProfile from "../ProfileCard/AuthorProfile";
import { useLocation, useNavigate } from "react-router-dom";
import { HorizontalReactBar } from "../../molecules/Reaction";
import styled from "styled-components";
import { ActionButton } from "../../molecules/ButtonGroups";
import { PanelHeading, PanelDefault, PanelBody } from "../../molecules/Panels";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import ImageThumb from "../../molecules/Images/ImageThumb";
import { secondaryTitleLink } from "../../atoms/Titles/TitleLinks";
import { AnchorLink } from "../../atoms/Links";
import { HorizontalList } from "../../molecules/List";
import { FontButtonItem } from "../../molecules/ActionIcons";
import { PriceLabel } from "../../molecules/PriceAndCurrency";
import { convertDateTimeToPeriod } from "../../../utils/DateTimeUtils";
import ContentItemDropdown from "../../molecules/DropdownButton/ContentItemDropdown";
import ModuleMenuListItem from "../../molecules/MenuList/ModuleMenuListItem";
import { SessionContext } from "../../../store/context/session-context";
import DeleteConfirmationModal from "../Modals/DeleteConfirmationModal";

const Panel = styled(PanelDefault)`
  position: relative;
  margin-bottom: ${(p) => p.theme.size.distance};

  .no-image {
    height: 100%;
  }
`;

const PanelHeader = styled(PanelHeading)`
  padding-bottom: 0;
`;

const ContentTopbar = styled.div`
  margin-bottom: ${(p) => p.theme.size.exSmall};
  position: relative;

  ${ContentItemDropdown} {
    z-index: 10;
  }

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

const Title = styled(secondaryTitleLink)`
  margin-bottom: 0;

  svg {
    margin: 0 ${(p) => p.theme.size.exTiny};
  }
`;

const PostActions = styled.div`
  text-align: right;
`;

const Description = styled.p`
  margin-bottom: 0;
`;

const InteractiveToolbar = styled.div`
  border-top: 1px solid ${(p) => p.theme.color.neutralBorder};
  padding: ${(p) => p.theme.size.exSmall} ${(p) => p.theme.size.distance};
`;

const InteractiveItem = styled.li`
  margin-right: ${(p) => p.theme.size.small};
  :last-child {
    margin-right: 0;
  }
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

interface Props {
  product?: any;
  onOpenDeleteConfirmationModal: (e: any) => void;
}

const ProductListItem = (props: Props) => {
  const location = useLocation();
  const navigate = useNavigate();
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
    navigate(`/products/update/${product.id}`, {
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
      const datePeriod = convertDateTimeToPeriod(product.createdDate);
      creator.info = (
        <Fragment>
          <FontAwesomeIcon icon="calendar-alt" />
          <span>{datePeriod}</span>
        </Fragment>
      );
    }
    return creator;
  };

  const onOpenDeleteConfirmation = () => {
    onOpenDeleteConfirmationModal({
      title: "Delete Farm",
      innerModal: DeleteConfirmationModal,
      message: `Are you sure to delete product "${product.name}"?`,
      id: parseFloat(product.id),
    });
  };

  return (
    <div className="px-1">
      <Panel>
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

          <Title>
            {product.farms
              ? product.farms.map((pf: any) => {
                  if (!pf.id) {
                    return null;
                  }
                  return (
                    <Fragment key={pf.id}>
                      <AnchorLink
                        to={{
                          pathname: pf.url,
                          state: { from: location.pathname },
                        }}
                      >
                        {pf.name}
                      </AnchorLink>
                      <FontAwesomeIcon icon="angle-right" />
                    </Fragment>
                  );
                })
              : null}

            <AnchorLink
              to={{
                pathname: product.url,
                state: { from: location.pathname },
              }}
            >
              {product.name}
            </AnchorLink>
          </Title>
        </PanelHeader>
        <PanelBody>
          <div className="row">
            <div className="col col-6 col-sm-6 col-md-5 col-lg-5">
              <AnchorLink
                to={{
                  pathname: product.url,
                  state: { from: location.pathname },
                }}
              >
                <ImageThumb src={product.pictureUrl} alt="" />
              </AnchorLink>
            </div>

            <div className="col col-6 col-sm-6 col-md-7 col-lg-7">
              {product.price > 0 ? (
                <PriceLabel price={product.price} currency="vnđ" />
              ) : null}

              <div className="panel-content">
                <Description>
                  <span
                    dangerouslySetInnerHTML={{ __html: product.description }}
                  ></span>{" "}
                </Description>
              </div>
            </div>
          </div>
        </PanelBody>
        {/* <InteractiveToolbar>
          <HorizontalList>
            <InteractiveItem>
              <HorizontalReactBar
                reactionNumber={product.reactionNumber}
                className="me-3"
              />
              <FontButtonItem
                icon="comments"
                dynamicText={product.commentNumber}
                title="Discussions"
              />
            </InteractiveItem>
            <InteractRightItem>
              <FontButtonItem
                className="add-to-cart"
                icon="shopping-bag"
                title="Thêm vô giỏ hàng"
              />
            </InteractRightItem>
          </HorizontalList>
        </InteractiveToolbar> */}
      </Panel>
    </div>
  );
};

export default ProductListItem;
