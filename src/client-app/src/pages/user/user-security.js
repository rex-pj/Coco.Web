import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@apollo/client";
import PasswordUpdateForm from "../../components/organisms/Profile/PasswordUpdateForm";
import { userMutations } from "../../graphql/fetching/mutations";
import { useStore } from "../../store/hook-store";
import ConfirmToRedirectModal from "../../components/organisms/Modals/ConfirmToRedirectModal";

export default (props) => {
  const navigate = useNavigate();
  const [isFormEnabled, setFormEnabled] = useState(true);
  const dispatch = useStore(false)[1];
  const [updateUserPassword] = useMutation(userMutations.UPDATE_USER_PASSWORD);
  const { canEdit } = props;

  const onUpdateConfirmation = () => {
    dispatch("OPEN_MODAL", {
      data: {
        title: "You will need to log out and log in again",
        children:
          "To make sure all functions are working properly you need to log out and log in again",
        executeButtonName: "Ok",
      },
      execution: {
        onSucceed: onSucceed,
      },
      options: {
        isOpen: true,
        innerModal: ConfirmToRedirectModal,
        unableClose: true,
      },
    });
  };

  const onSucceed = () => {
    navigate("/auth/logout");
  };

  const showNotification = (title, message, type) => {
    dispatch("NOTIFY", {
      title,
      message,
      type: type,
    });
  };

  const onUpdatePassword = async (data) => {
    if (!canEdit) {
      return;
    }

    setFormEnabled(true);

    if (updateUserPassword) {
      await updateUserPassword({
        variables: {
          criterias: data,
        },
      })
        .then((response) => {
          const { errors } = response;
          if (errors) {
            setFormEnabled(true);
            showNotification(
              "An error occured when update the password",
              "Please check your input and try again",
              "error"
            );
          }

          showNotification(
            "The password is changed successfully",
            "The password is changed successfully",
            "info"
          );
          onUpdateConfirmation();
          setFormEnabled(true);
        })
        .catch((error) => {
          setFormEnabled(true);
          showNotification(
            "An error occured when update the password",
            "Please check your input and try again",
            "error"
          );
        });
    }
  };

  return (
    <PasswordUpdateForm
      onUpdate={(e) => onUpdatePassword(e, canEdit)}
      isFormEnabled={isFormEnabled}
      canEdit={canEdit}
      showValidationError={props.showValidationError}
    />
  );
};
