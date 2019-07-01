import React, { useState, Fragment } from "react";
import styled from "styled-components";
import { Selection } from "../../atoms/Selections";

const TextLabel = styled.span`
  display: inline-block;

  &.can-edit {
    border-bottom: 1px dashed ${p => p.theme.color.normal};
    line-height: ${p => p.theme.size.normal};
    height: ${p => p.theme.size.normal};
    cursor: pointer;
    min-width: calc(${p => p.theme.size.large} * 2);
  }

  &.empty {
    color: ${p => p.theme.color.danger};
    font-weight: 400;
  }
`;

const SelectBox = styled(Selection)`
  min-width: calc(${p => p.theme.size.large} * 2);
  cursor: pointer;
  border: 0;
  border-bottom: 1px dashed ${p => p.theme.color.normal};
  border-radius: 0;
`;

function Options(props) {
  const { selections, emptyText } = props;
  return selections ? (
    <Fragment>
      <option value={0} disabled={true}>
        {emptyText}
      </option>
      {selections.map(item => (
        <option key={item.id} value={item.id}>
          {item.text}
        </option>
      ))}
    </Fragment>
  ) : null;
}

export default function(props) {
  const { selections, value, name, disabled } = props;
  let emptyText = "---Select---";
  if (props.emptyText) {
    emptyText = props.emptyText;
  }

  const current =
    value && selections
      ? selections.find(item => item.id.toString() === value.toString())
      : { id: 0, text: "Not selected" };

  const [selectedValue, updateSelectedValue] = useState({
    id: current ? current.id : null,
    text: current ? current.text : null
  });

  function onChanged(e) {
    const { name, primaryKey } = props;
    const currentValue = selections
      ? selections.find(function(element) {
          return element.id.toString() === e.target.value;
        })
      : { id: 0, text: emptyText };

    if (currentValue) {
      updateSelectedValue(currentValue);
      if (props.onUpdated) {
        props
          .onUpdated({
            primaryKey,
            value: currentValue.id,
            propertyName: name
          })
          .then(function(response) {})
          .catch(function(errors) {
            const oldValue = selections
              ? selections.find(function(element) {
                  return element.id.toString() === value.toString();
                })
              : { id: 0, text: emptyText };

            console.log(errors);
            updateSelectedValue({
              id: value,
              text: oldValue.text
            });
          });
      }
    } else {
      updateSelectedValue({ id: 0, text: emptyText });
    }
  }

  if (!props.disabled && !!selectedValue) {
    return (
      <SelectBox
        name={name}
        disabled={disabled}
        placeholder={emptyText}
        value={selectedValue.id}
        onChange={onChanged}
      >
        <Options selections={selections} emptyText={emptyText} />
      </SelectBox>
    );
  } else if (!props.disabled && value) {
    return (
      <SelectBox
        name={name}
        disabled={disabled}
        placeholder={emptyText}
        onChange={props.onChanged}
        value={value}
      >
        <Options selections={selections} emptyText={emptyText} />
      </SelectBox>
    );
  } else if (!props.disabled) {
    return (
      <SelectBox
        name={name}
        disabled={disabled}
        placeholder={emptyText}
        onChange={props.onChanged}
      >
        <Options selections={selections} emptyText={emptyText} />
      </SelectBox>
    );
  } else {
    return (
      <TextLabel className="disabled">
        {selectedValue ? selectedValue.text : emptyText}
      </TextLabel>
    );
  }
}