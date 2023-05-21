import * as React from "react";
import { useState, useRef, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { SecondaryTextbox } from "../../atoms/Textboxes";
import { ButtonSecondary } from "../../atoms/Buttons/Buttons";
import { checkValidity } from "../../../utils/Validity";
import styled from "styled-components";
import {
  ImageUpload,
  ImageUploadOnChangeEvent,
} from "../UploadControl/ImageUpload";
import AsyncSelect from "react-select/async";
import { ArticleCreationModel } from "../../../models/articleCreationModel";
import { Thumbnail } from "../../molecules/Thumbnails";
import { mapSelectOptions } from "../../../utils/SelectOptionUtils";
import { apiConfig } from "../../../config/api-config";
import { ActionMeta, OnChangeValue } from "react-select";
import { LexicalEditor } from "lexical";
import { $generateHtmlFromNodes } from "@lexical/html";
import { LexicalComposer } from "@lexical/react/LexicalComposer";
import { SharedHistoryContext } from "../CommonEditor/context/SharedHistoryContext";
import { TableContext } from "../CommonEditor/plugins/TablePlugin";
import { SharedAutocompleteContext } from "../CommonEditor/context/SharedAutocompleteContext";
import { RichTextEditor } from "../CommonEditor/RichTextEditor";
import { defaultEditorConfigs } from "../CommonEditor/configs";
import AsyncSubmitFormPlugin from "../CommonEditor/plugins/AsyncSubmitFormPlugin";

const FormRow = styled.div`
  margin-bottom: ${(p) => p.theme.size.tiny};

  ${SecondaryTextbox} {
    width: 100%;
  }

  .cate-selection {
    z-index: 10;

    > div {
      border: 1px solid ${(p) => p.theme.color.secondaryBg};
    }
  }
`;

const ThumbnailUpload = styled(ImageUpload)`
  text-align: center;
  margin: auto;
  display: inline-block;
  vertical-align: middle;

  > span {
    color: ${(p) => p.theme.color.primaryText};
    height: ${(p) => p.theme.size.normal};
    padding: 0 ${(p) => p.theme.size.tiny};
    font-size: ${(p) => p.theme.fontSize.tiny};
    background-color: ${(p) => p.theme.color.lightBg};
    border-radius: ${(p) => p.theme.borderRadius.normal};
    border: 1px solid ${(p) => p.theme.color.secondaryBg};
    cursor: pointer;
    font-weight: 600;

    :hover {
      background-color: ${(p) => p.theme.color.secondaryBg};
    }

    svg {
      display: inline-block;
      margin: 10px auto 0 auto;
    }
  }
`;

const ImageEditBox = styled.div`
  position: relative;
`;

const RemoveImageButton = styled.span`
  position: absolute;
  top: -${(p) => p.theme.size.exSmall};
  right: -${(p) => p.theme.size.exTiny};
  cursor: pointer;
`;

const Footer = styled.div`
  ${ButtonSecondary} {
    width: 200px;
  }
`;

interface ArticleEditorProps {
  height?: number;
  filterCategories: (e: any) => Promise<any>;
  currentArticle?: any;
  showValidationError: (title: string, message: string) => void;
  onArticlePost: (e: any) => Promise<any>;
}

const ArticleEditor: React.FC<ArticleEditorProps> = (props) => {
  const { filterCategories, currentArticle } = props;
  const [isSubmitted, setSubmitted] = useState(false);
  const [formData, setFormData] = useState(new ArticleCreationModel());
  const [preview, setPreview] = useState<string>("");
  const selectRef = useRef<any>();

  const handleInputChange = (evt: React.ChangeEvent<HTMLInputElement>) => {
    let data = formData || {};
    const { name, value } = evt.target;

    data[name].isValid = checkValidity(data, value, name);
    data[name].value = value;

    setFormData({
      ...data,
    });
  };

  const handleImageChange = (e: ImageUploadOnChangeEvent) => {
    const data = formData || new ArticleCreationModel();
    const { preview: srcPreview, file } = e;
    data["picture"].isValid = checkValidity(data, file, "picture");
    data["picture"].value = {
      file,
    };

    setPreview(srcPreview ?? "");
    setFormData({
      ...data,
    });
  };

  const onArticlePost: (
    e: React.FormEvent<HTMLFormElement>
  ) => Promise<any> = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setSubmitted(true);
    let isFormValid = true;
    for (let formIdentifier in formData) {
      isFormValid = formData[formIdentifier].isValid && isFormValid;
      break;
    }

    if (!isFormValid) {
      props.showValidationError(
        "Something went wrong with your input",
        "Something went wrong with your information, please check and input again"
      );

      setSubmitted(false);
      return Promise.reject("validation: Something went wrong with your input");
    }

    let articleForm: any = {};
    for (const formIdentifier in formData) {
      articleForm[formIdentifier] = formData[formIdentifier].value;
    }

    if (!articleForm.id) {
      delete articleForm["id"];
    }

    const requestData = new FormData();
    for (const key of Object.keys(articleForm)) {
      if (key === "picture" && articleForm[key]) {
        const picture = articleForm[key];
        requestData.append(`picture.file`, picture.file);
        if (picture.pictureId) {
          requestData.append(`file.pictureId`, picture.pictureId);
        }
      }
      if (key === "picture" && articleForm[key]) {
        const picture = articleForm[key];
        if (picture.pictureId) {
          requestData.append(`file.pictureId`, picture.pictureId);
        }
      } else {
        requestData.append(key, articleForm[key]);
      }
    }

    delete articleForm["articleCategoryName"];
    return await props
      .onArticlePost(requestData)
      .then((response: any) => {
        clearFormData();
        setSubmitted(false);
        return Promise.resolve(response);
      })
      .catch((error) => {
        setSubmitted(false);
        return Promise.reject(error);
      });
  };

  const clearFormData = () => {
    selectRef.current.clearValue();
    const articleFormData = { ...new ArticleCreationModel() };
    setPreview("");
    setFormData({ ...articleFormData });
  };

  const onImageRemoved = () => {
    let data = formData || {};
    data["picture"].isValid = false;
    data["picture"].value = null;

    setPreview("");
    setFormData({
      ...data,
    });
  };

  const loadCategorySelections = (value: string) => {
    return filterCategories({
      variables: {
        criterias: { query: value, isParentOnly: true },
      },
    })
      .then((response: any) => {
        const { data } = response;
        const { selections } = data;
        return mapSelectOptions(selections);
      })
      .catch((error) => {
        return [];
      });
  };

  const handleSelectChange = (
    newValue: OnChangeValue<any, any>,
    actionMeta: ActionMeta<any>
  ) => {
    const { action } = actionMeta;
    let data = formData || {};
    if (action === "clear" || action === "remove-value") {
      data.articleCategoryId.isValid = false;
      data.articleCategoryId.value = 0;
      data.articleCategoryName.value = "";
    } else {
      const { value, label } = newValue;
      data.articleCategoryId.isValid = checkValidity(
        data,
        value,
        "articleCategoryId"
      );
      data.articleCategoryId.value = parseFloat(value);
      data.articleCategoryName.value = label;
    }

    setFormData({
      ...data,
    });
  };

  const loadCategorySelected = () => {
    const { articleCategoryName, articleCategoryId } = formData;
    if (!articleCategoryId.value) {
      return null;
    }
    return {
      label: articleCategoryName.value,
      value: articleCategoryId.value,
    };
  };

  const onContentChanged = (editor: LexicalEditor) => {
    const html = $generateHtmlFromNodes(editor, null);
    let data = formData || new ArticleCreationModel();
    data.content.isValid = checkValidity(data, html, "content");
    data.content.value = html;
    setFormData({
      ...data,
    });
  };

  useEffect(() => {
    if (currentArticle && !formData?.id?.value) {
      const { picture } = currentArticle;
      if (picture?.value?.pictureId) {
        setPreview(
          `${apiConfig.paths.pictures.get.getPicture}/${picture.value.pictureId}`
        );
      }

      setFormData({ ...currentArticle });
    }
  }, [currentArticle, formData]);

  const { name, articleCategoryId, content } = formData;
  const htmlContent = content?.value;
  return (
    <LexicalComposer initialConfig={defaultEditorConfigs}>
      <SharedHistoryContext>
        <TableContext>
          <SharedAutocompleteContext>
            <AsyncSubmitFormPlugin
              onSubmitAsync={(e) => onArticlePost(e)}
              method="POST"
              clearAfterSubmit={true}
            >
              <FormRow className="row">
                <div className="col-12 col-lg-6 pr-lg-1 mb-2 mb-lg-0">
                  <SecondaryTextbox
                    name="name"
                    defaultValue={name.value}
                    autoComplete="off"
                    onChange={(e) => handleInputChange(e)}
                    placeholder="Post title"
                  />
                </div>
                <div className="col-10 col-lg-4 px-lg-1 pr-1 me-auto">
                  <AsyncSelect
                    key={JSON.stringify(articleCategoryId)}
                    className="cate-selection"
                    ref={selectRef}
                    defaultValue={loadCategorySelected()}
                    cacheOptions
                    defaultOptions
                    onChange={handleSelectChange}
                    loadOptions={loadCategorySelections}
                    isClearable={true}
                  />
                </div>
                <div className="col">
                  <ThumbnailUpload
                    onChange={handleImageChange}
                  ></ThumbnailUpload>
                </div>
              </FormRow>
              {preview ? (
                <FormRow className="row">
                  <div className="col-3">
                    <ImageEditBox>
                      <Thumbnail src={preview}></Thumbnail>
                      <RemoveImageButton onClick={onImageRemoved}>
                        <FontAwesomeIcon icon="times"></FontAwesomeIcon>
                      </RemoveImageButton>
                    </ImageEditBox>
                  </div>
                </FormRow>
              ) : null}

              <div className="editor-shell">
                <RichTextEditor
                  initialHtml={htmlContent}
                  onChange={onContentChanged}
                />
              </div>

              <Footer className="row mb-3">
                <div className="col-auto"></div>
                <div className="col-auto ms-auto">
                  <ButtonSecondary disabled={isSubmitted} size="xs">
                    Post
                  </ButtonSecondary>
                </div>
              </Footer>
            </AsyncSubmitFormPlugin>
          </SharedAutocompleteContext>
        </TableContext>
      </SharedHistoryContext>
    </LexicalComposer>
  );
};

export default ArticleEditor;