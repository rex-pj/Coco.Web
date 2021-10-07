import React from "react";
import styled from "styled-components";
import { ImageRound } from "../../atoms/Images";
import { Thumbnail } from "../../molecules/Thumbnails";
import Menubar from "./CardToolbar";
import Overlay from "../../atoms/Overlay";
import { AnchorLink } from "../../atoms/Links";
import NoAvatar from "../../molecules/NoImages/no-avatar";
import NoImage from "../../molecules/NoImages/no-image";

const Root = styled.div`
  position: relative;
`;

const CoverWrapper = styled.div`
  max-height: 110px;
  overflow: hidden;
  position: relative;

  img {
    border-top-left-radius: ${(p) => p.theme.borderRadius.normal};
    border-top-right-radius: ${(p) => p.theme.borderRadius.normal};
  }
`;

const Username = styled.h3`
  margin-bottom: 0;
  position: absolute;
  bottom: calc(${(p) => p.theme.size.normal} + 10px);
  left: ${(p) => p.theme.size.exSmall};
  z-index: 1;
  line-height: 1;
  font-size: ${(p) => p.theme.fontSize.normal};
  text-shadow: ${(p) => p.theme.shadow.TextShadow};
  a {
    color: ${(p) => p.theme.color.lightText};
  }
`;

const ProfileImage = styled(ImageRound)`
  position: absolute;
  top: -20px;
  left: ${(p) => p.theme.size.exSmall};
  width: calc(${(p) => p.theme.size.medium} + ${(p) => p.theme.size.exTiny});
  height: calc(${(p) => p.theme.size.medium} + ${(p) => p.theme.size.exTiny});
  border: 3px solid ${(p) => p.theme.rgbaColor.cyan};
  z-index: 1;
`;

const EmptyAvatar = styled(NoAvatar)`
  border-radius: ${(p) => p.theme.borderRadius.normal};
  width: 55px;
  height: 55px;
  font-size: 24px;
  position: absolute;
  top: -20px;
  left: 15px;
  border: 3px solid ${(p) => p.theme.rgbaColor.cyan};
  z-index: 1;
`;

const EmptyCover = styled(NoImage)`
  height: 60px;
`;

const BoxShadowBar = styled.div`
  position: relative;
`;

const StaticBar = styled.div`
  height: ${(p) => p.theme.size.medium};
  border-bottom-left-radius: ${(p) => p.theme.borderRadius.normal};
  border-bottom-right-radius: ${(p) => p.theme.borderRadius.normal};
`;

export default function (props) {
  const { className, menuList } = props;
  const { userInfo } = props;
  const userIdentityId = userInfo?.userIdentityId;

  return (
    <Root className={className}>
      {userInfo && userInfo.userAvatar && userInfo.userAvatar.code ? (
        <ProfileImage
          src={`${process.env.REACT_APP_CDN_AVATAR_API_URL}${userInfo.userAvatar.code}`}
        />
      ) : (
        <EmptyAvatar />
      )}

      <BoxShadowBar>
        <CoverWrapper>
          {userInfo && userInfo.userCover && userInfo.userCover.code ? (
            <Thumbnail
              src={`${process.env.REACT_APP_CDN_COVER_PHOTO_API_URL}${userInfo.userCover.code}`}
            />
          ) : (
            <EmptyCover />
          )}
          <Overlay />
        </CoverWrapper>
        <Username>
          <AnchorLink to={userIdentityId ? `/profile/${userIdentityId}` : ""}>
            {userInfo ? userInfo.displayName : ""}
          </AnchorLink>
        </Username>
        <StaticBar>
          <Menubar menuList={menuList} />
        </StaticBar>
      </BoxShadowBar>
    </Root>
  );
}
