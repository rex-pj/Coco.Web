import * as React from "react";
import styled from "styled-components";
import { RouterLinkButtonPrimary } from "../../atoms/Buttons/RouterLinkButtons";
import { HorizontalList } from "../../molecules/List";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useTranslation } from "react-i18next";

const Root = styled.div`
  background-color: ${(p) => p.theme.color.secondaryBg};
  position: relative;
`;

const NavButton = styled(RouterLinkButtonPrimary)`
  color: ${(p) => p.theme.color.neutralText};
  font-weight: 500;
  font-size: ${(p) => p.theme.fontSize.small};
  border: 0;
  border-top-right-radius: ${(p) => p.theme.borderRadius.medium};
  border-top-left-radius: ${(p) => p.theme.borderRadius.medium};
  border-bottom-left-radius: 0;
  border-bottom-right-radius: 0;
  background-color: transparent;

  :hover {
    color: ${(p) => p.theme.color.lightText};
    background-color: transparent;
  }
`;

const ListItem = styled.li`
  display: inline-block;

  &.actived ${NavButton} {
    background-color: ${(p) => p.theme.color.primaryBg};
  }

  &.actived span {
    display: block;
    color: ${(p) => p.theme.color.neutralText};
    background-color: ${(p) => p.theme.color.primaryBg};
    font-weight: 500;
    font-size: ${(p) => p.theme.fontSize.small};
    border: 0;
    border-top-right-radius: ${(p) => p.theme.borderRadius.medium};
    border-top-left-radius: ${(p) => p.theme.borderRadius.medium};
    border-bottom-left-radius: 0;
    border-bottom-right-radius: 0;
    padding: ${(p) => (p.size === "sm" ? ".5rem .75rem" : "10px 15px")};
  }

  :first-child ${NavButton} {
    border-top-left-radius: 0;
  }
`;

type ResetPasswordNavigationProps = {
  className?: string;
};

const ResetPasswordNavigation = (props: ResetPasswordNavigationProps) => {
  const { t } = useTranslation();
  const { className } = props;
  return (
    <Root>
      <HorizontalList className={className}>
        <ListItem>
          <NavButton to="/">
            <FontAwesomeIcon icon="home" />
          </NavButton>
        </ListItem>
        <ListItem className="actived">
          <span>{t("reset_password_title")}</span>
        </ListItem>
      </HorizontalList>
    </Root>
  );
};

export default ResetPasswordNavigation;
