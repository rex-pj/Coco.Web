import React, { useState } from "react";
import styled from "styled-components";
import { PrimaryTextbox } from "../../atoms/Textboxes";
import {
  ButtonOutlinePrimary,
  ButtonOutlineLight,
} from "../../atoms/Buttons/OutlineButtons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const Wrap = styled.div`
  button {
    margin-left: ${(p) => p.theme.size.exTiny};
  }
`;

const TextLabel = styled.span`
  display: inline-block;

  &.can-edit {
    border-bottom: 1px dashed ${(p) => p.theme.color.neutralBg};
    line-height: ${(p) => p.theme.size.normal};
    height: ${(p) => p.theme.size.normal};
  }

  &.empty {
    color: ${(p) => p.theme.color.warnText};
    font-weight: 400;
  }
`;

const TextEditing = styled(PrimaryTextbox)`
  border: 0;
  border-bottom: 1px dashed ${(p) => p.theme.color.neutralBg};
  border-radius: 0;
`;

export default function (props) {
  const [isOpen, setIsOpen] = useState(false);
  const [value, setValue] = useState(props.value ? props.value : "");

  function openTextBox() {
    setIsOpen(true);
  }

  function closeTextBox() {
    setIsOpen(false);
  }

  async function onClickUpdate() {
    await pushData();
  }

  async function onEnterUpdate(e) {
    if (e.keyCode === 13 && props.onUpdated) {
      await pushData();
    } else if (e.keyCode === 13) {
      closeTextBox();
      setValue(value);
    } else if (e.keyCode === 27) {
      closeTextBox();
      const currentValue = props.value ? props.value : "";
      setValue(currentValue);
    }
  }

  async function pushData() {
    const currentValue = props.value ? props.value : "";

    if (!!props.primaryKey && !!props.name && currentValue !== value) {
      await props
        .onUpdated({
          primaryKey: props.primaryKey,
          value: value,
          propertyName: props.name,
        })
        .then(function (response) {
          if (response) {
            const { data } = response;
            const { updateUserInfoItem } = data;
            const { result } = updateUserInfoItem;
            setValue(result.value);
          } else {
            setValue(currentValue);
          }
        })
        .catch(function (errors) {
          setValue(currentValue);
        });
    }

    closeTextBox();
  }

  function onBlur() {
    closeTextBox();
    setValue(value);
  }

  function onChange(e) {
    setValue(e.target.value);
  }

  let emptyText = "Empty";
  if (props.emptyText) {
    emptyText = props.emptyText;
  }

  if (!props.disabled && !!isOpen && props.enterByKey) {
    return (
      <TextEditing
        name={props.name}
        value={value ? value : ""}
        onBlur={onBlur}
        autoFocus={true}
        onKeyUp={onEnterUpdate}
        onChange={onChange}
      />
    );
  } else if (!props.disabled && !!isOpen) {
    return (
      <Wrap>
        <TextEditing
          name={props.name}
          value={value ? value : ""}
          autoFocus={true}
          onChange={onChange}
        />{" "}
        <ButtonOutlineLight size="xs" onClick={onClickUpdate}>
          <FontAwesomeIcon icon="check" />
        </ButtonOutlineLight>
        <ButtonOutlinePrimary size="xs" onClick={closeTextBox}>
          <FontAwesomeIcon icon="times" />
        </ButtonOutlinePrimary>
      </Wrap>
    );
  }

  if (!value && !props.disabled) {
    return (
      <TextLabel className="can-edit empty" onClick={openTextBox}>
        {emptyText}
      </TextLabel>
    );
  }

  return !props.disabled ? (
    <TextLabel className="can-edit" onClick={openTextBox}>
      {value}
    </TextLabel>
  ) : (
    <TextLabel className="disabled">{value}</TextLabel>
  );
}
