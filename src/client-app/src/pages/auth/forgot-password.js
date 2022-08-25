import React, { useContext } from "react";
import ForgotPasswordForm from "../../components/organisms/Auth/ForgotPasswordForm";
import { useMutation } from "@apollo/client";
import { unauthClient } from "../../graphql/client";
import { userMutations } from "../../graphql/fetching/mutations";
import { getError } from "../../utils/Helper";
import { SessionContext } from "../../store/context/session-context";
import { useStore } from "../../store/hook-store";
import { AuthLayout } from "../../components/templates/Layout";

export default (props) => {
  const dispatch = useStore(false)[1];
  const { lang } = useContext(SessionContext);
  const [forgotPassword] = useMutation(userMutations.FORGOT_PASSWORD, {
    client: unauthClient,
  });

  const notifyError = (error, lang) => {
    if (
      error &&
      error.networkError &&
      error.networkError.result &&
      error.networkError.result.errors
    ) {
      const errors = error.networkError.result.errors;

      errors.forEach((item) => {
        dispatch("NOTIFY", {
          title: "Something went wrong when update your password",
          message: getError(item.extensions.code, lang),
          type: "error",
        });
      });
    } else {
      dispatch("NOTIFY", {
        title: "Something went wrong when update your password",
        message: getError("ErrorOccurredTryRefeshInputAgain", lang),
        type: "error",
      });
    }
  };

  const showValidationError = (title, message) => {
    dispatch("NOTIFY", {
      title,
      message,
      type: "error",
    });
  };

  const onForgotPassword = async (data) => {
    return await forgotPassword({
      variables: {
        criterias: data,
      },
    })
      .then((response) => {
        const { data } = response;
        const { forgotPassword: rs } = data;

        if (!rs || !rs.isSucceed) {
          notifyError(rs.errors, lang);
        }
      })
      .catch((error) => {
        notifyError(error, lang);
      });
  };

  return (
    <AuthLayout>
      <ForgotPasswordForm
        onForgotPassword={(data) => onForgotPassword(data, forgotPassword)}
        showValidationError={showValidationError}
      />
    </AuthLayout>
  );
};
