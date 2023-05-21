import * as React from "react";
import styled from "styled-components";
import { ButtonPrimary } from "../../atoms/Buttons/Buttons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { IconProp } from "@fortawesome/fontawesome-svg-core";

const Text = styled.span`
  margin-left: ${(p) => p.theme.size.exTiny};
  color: inherit;
`;

interface ActionButtonProps {
  readonly icon?: IconProp;
  readonly children: any;
  className?: string;
  size?: string;
}

const ButtonIconPrimary = (props: ActionButtonProps) => {
  const { icon, children, className, size } = props;
  return (
    <ButtonPrimary className={className} size={size}>
      {icon ? <FontAwesomeIcon icon={icon} /> : null}
      <Text>{children}</Text>
    </ButtonPrimary>
  );
};

export { ButtonIconPrimary };