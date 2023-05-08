import * as React from "react";
import { useState, useRef, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import styled from "styled-components";
import { ButtonPrimary, ButtonLight } from "../../atoms/Buttons/Buttons";
import { PrimaryTextbox } from "../../atoms/Textboxes";
import { LabelNormal } from "../../atoms/Labels";
import { checkValidity, validateLink } from "../../../utils/Validity";
import { EditorState, Modifier } from "draft-js";
import { getEntityRange, getSelectionEntity } from "draftjs-utils";
import { DefaultEditorLink, EditorLinkModel } from "./models/EditorLinkModel";

const Root = styled.div`
  max-width: 80%;
  width: 400px;
  background: ${(p) => p.theme.color.whiteBg};
  margin: ${(p) => p.theme.size.exTiny} auto 0 auto;
  border-radius: ${(p) => p.theme.borderRadius.normal};
  box-shadow: ${(p) => p.theme.shadow.BoxShadow};
`;

const Body = styled.div`
  height: auto;
  padding: ${(p) => p.theme.size.tiny} ${(p) => p.theme.size.distance};
`;

const Footer = styled.div`
  min-height: 20px;
  text-align: right;
  border-top: 1px solid ${(p) => p.theme.color.neutralBg};
  padding: ${(p) => p.theme.size.exTiny} ${(p) => p.theme.size.distance};

  button {
    margin-left: ${(p) => p.theme.size.exTiny};
    font-weight: normal;
    padding: ${(p) => p.theme.size.exTiny} ${(p) => p.theme.size.tiny};

    svg {
      margin-right: ${(p) => p.theme.size.exTiny};
    }
  }
`;

const FormRow = styled.div`
  margin-bottom: ${(p) => p.theme.size.tiny};

  ${LabelNormal} {
    display: block;
  }

  ${PrimaryTextbox} {
    width: 100%;
  }
`;

export interface EditorLinkModalAcceptEvent {
  newEditorState: EditorState;
  entityKey: string;
}

interface EditorLinkModalProps {
  isOpen: boolean;
  editorState: EditorState;
  currentValue: { link: { title: string; target: any }; selectionText: string };
  onClose: () => void;
  onAccept: (e: EditorLinkModalAcceptEvent) => void;
}

export const EditorLinkModal: React.FC<EditorLinkModalProps> = (props) => {
  const { isOpen, editorState, currentValue } = props;
  const linkRef = useRef<HTMLInputElement>();
  const { link, selectionText } = currentValue;
  const linkText = link && link.title ? link.title : "";
  const isLinkValid = validateLink(link ? link.target : "");

  const formData: EditorLinkModel = {
    url: {
      value: link && link.target ? link.target : "",
      validation: {
        isRequired: true,
        isLink: true,
      },
      isValid: isLinkValid,
    },
    title: {
      value: selectionText ? selectionText : linkText,
      validation: {
        isRequired: false,
      },
      isValid: true,
    },
  };

  const [linkData, setLinkData] = useState<EditorLinkModel>(formData);

  const handleInputChange = (evt: React.ChangeEvent<HTMLInputElement>) => {
    let data = linkData || DefaultEditorLink;
    const { name, value } = evt.target;

    data[name].isValid = checkValidity(data, value, name);
    data[name].value = value;

    setLinkData({
      ...data,
    });
  };

  const onClose = () => {
    props.onClose();
  };

  const handleKeyUp = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      onAddLink();
    }
  };

  const onAccept = (
    linkTitle: string,
    linkTarget: any,
    linkTargetOption: any
  ) => {
    const currentEntity = editorState
      ? getSelectionEntity(editorState)
      : undefined;
    let selection = editorState.getSelection();

    if (currentEntity) {
      // TODO: Replace to other editor
      // const entityRange = getEntityRange(editorState, currentEntity);
      // const isBackward = selection.getIsBackward();
      // if (isBackward) {
      //   selection = selection.merge({
      //     anchorOffset: entityRange.end,
      //     focusOffset: entityRange.start,
      //   });
      // } else {
      //   selection = selection.merge({
      //     anchorOffset: entityRange.start,
      //     focusOffset: entityRange.end,
      //   });
      // }
    }

    const entityKey = editorState
      .getCurrentContent()
      .createEntity("LINK", "MUTABLE", {
        url: linkTarget,
        targetOption: linkTargetOption,
      })
      .getLastCreatedEntityKey();

    let contentState = Modifier.replaceText(
      editorState.getCurrentContent(),
      selection,
      `${linkTitle}`,
      editorState.getCurrentInlineStyle(),
      entityKey
    );

    let newEditorState = EditorState.push(
      editorState,
      contentState,
      "insert-characters"
    );

    // insert a blank space after link
    selection = newEditorState.getSelection().merge({
      anchorOffset: selection.get("anchorOffset") + linkTitle.length,
      focusOffset: selection.get("anchorOffset") + linkTitle.length,
    });

    newEditorState = EditorState.acceptSelection(newEditorState, selection);
    contentState = Modifier.insertText(
      newEditorState.getCurrentContent(),
      selection,
      " ",
      newEditorState.getCurrentInlineStyle(),
      undefined
    );

    props.onAccept({
      newEditorState,
      entityKey: "insert-characters",
    });
  };

  const clear = () => {
    let data = formData || DefaultEditorLink;

    setLinkData({
      ...data,
    });
  };

  useEffect(() => {
    if (isOpen) {
      focusLinkInput();
    }
  }, [isOpen]);

  const focusLinkInput = () => {
    linkRef.current.focus();
  };

  const onAddLink = () => {
    const { url, title } = linkData;
    if (props.onAccept && url.isValid) {
      const linkTitle = title.value ? title.value : url.value;
      onAccept(linkTitle, url.value, {});
      clear();
      onClose();
    }
  };

  const { title, url } = linkData;
  const { isValid } = url;
  return (
    <Root>
      <Body>
        <FormRow>
          <LabelNormal>Tile</LabelNormal>
          <PrimaryTextbox
            name="title"
            onKeyUp={handleKeyUp}
            value={title.value}
            autoComplete="off"
            onChange={(e) => handleInputChange(e)}
          />
        </FormRow>
        <FormRow>
          <LabelNormal>Link</LabelNormal>
          <PrimaryTextbox
            name="url"
            onKeyUp={handleKeyUp}
            value={url.value}
            autoComplete="off"
            ref={linkRef}
            onChange={(e) => handleInputChange(e)}
          />
        </FormRow>
      </Body>
      <Footer>
        <ButtonLight size="sm" onClick={onClose}>
          Đóng
        </ButtonLight>
        <ButtonPrimary size="sm" onClick={onAddLink} disabled={!isValid}>
          <FontAwesomeIcon icon="check" />
          Lưu
        </ButtonPrimary>
      </Footer>
    </Root>
  );
};
