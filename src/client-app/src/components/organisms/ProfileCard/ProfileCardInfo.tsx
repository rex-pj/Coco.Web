import * as React from "react";
import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { VerticalList } from "../../molecules/List";
import { PanelBody } from "../../molecules/Panels";
import { LabelDark } from "../../atoms/Labels";
import { Fragment } from "react";
import { UrlConstant } from "../../../utils/Constants";
import { AnchorLink } from "../../atoms/Links";

const Root = styled.div`
  position: relative;
  border-radius: ${(p) => p.theme.borderRadius.normal};
`;

const Label = styled(LabelDark)`
  font-size: ${(p) => p.theme.fontSize.tiny};
  font-weight: 600;
`;

const InfoList = styled(VerticalList)`
  margin-bottom: ${(p) => p.theme.size.distance};
`;

const ChildItem = styled.li`
  font-size: ${(p) => p.theme.fontSize.small};
  color: ${(p) => p.theme.color.darkText};
  margin-bottom: ${(p) => p.theme.size.exTiny};

  span {
    color: inherit;
  }

  svg {
    margin-right: ${(p) => p.theme.size.exTiny};
  }

  svg,
  path {
    color: ${(p) => p.theme.color.darkText};
    font-size: ${(p) => p.theme.fontSize.tiny};
  }

  a {
    font-size: ${(p) => p.theme.fontSize.tiny};
    font-weight: 600;
  }
`;

interface Props {
  author?: any;
}

const ProfileCard = (props: Props) => {
  const { author } = props;
  let authorInfo = { ...author };
  if (!author) {
    authorInfo = {};
  }

  let farms = [];
  if (author) {
    farms = author.farms;
  }
  return (
    <Root>
      <PanelBody>
        <div>
          {authorInfo.description ? (
            <Fragment>
              <InfoList>
                <ChildItem>
                  <FontAwesomeIcon icon="quote-left" />
                  <span>{authorInfo.description}</span>
                </ChildItem>
              </InfoList>
            </Fragment>
          ) : null}
          <Label>Title</Label>
          <InfoList>
            {farms ? (
              <ChildItem>
                <FontAwesomeIcon icon="user" />
                <span>Farmer</span>
              </ChildItem>
            ) : (
              <ChildItem>
                <FontAwesomeIcon icon="user" />
                <span>Visitor</span>
              </ChildItem>
            )}
          </InfoList>

          {authorInfo.address ? (
            <Fragment>
              <Label>Address</Label>
              <InfoList>
                <ChildItem>
                  <FontAwesomeIcon icon="map-marker-alt" />
                  <span>{authorInfo.address}</span>
                </ChildItem>
              </InfoList>
            </Fragment>
          ) : null}

          {farms && farms.length > 0 ? (
            <InfoList>
              {farms.map((farm: any, index: number) => {
                const farmUrl = `${UrlConstant.Farm.url}${farm.id}`;
                return (
                  <ChildItem key={index}>
                    <FontAwesomeIcon icon="warehouse" />
                    <AnchorLink to={farmUrl}>{farm.name}</AnchorLink>
                  </ChildItem>
                );
              })}
            </InfoList>
          ) : null}
        </div>
      </PanelBody>
    </Root>
  );
};

export default ProfileCard;
