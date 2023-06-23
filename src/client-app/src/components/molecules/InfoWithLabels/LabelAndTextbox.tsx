import * as React from "react";
import styled from "styled-components";
import { PrimaryTextbox } from "../../atoms/Textboxes";
import { InputHTMLAttributes } from "react";

const ChildItem = styled.div`
  font-size: ${(p) => p.theme.fontSize.small};
  color: ${(p) => p.theme.color.darkText};
  margin-bottom: ${(p) => p.theme.size.tiny};
  padding-bottom: ${(p) => p.theme.size.tiny};
  min-height: ${(p) => p.theme.size.normal};

  label {
    color: ${(p) => p.theme.color.darkText};
    display: block;
    font-weight: 600;
  }
`;

interface Props extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
}

const LabelAndTextbox = React.forwardRef((props: Props, ref: any) => {
  const { className, value, defaultValue, label, name } = props;
  return (
    <ChildItem className={className}>
      {label ? <label>{label}</label> : null}
      <PrimaryTextbox
        ref={ref}
        value={value}
        defaultValue={defaultValue}
        onChange={props.onChange}
        name={name}
        autoComplete={props.autoComplete}
        placeholder={props.placeholder}
        type={props.type}
      />
    </ChildItem>
  );
});

export default LabelAndTextbox;
