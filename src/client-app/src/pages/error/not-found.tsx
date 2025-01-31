import * as React from "react";
import NotFound from "../../components/organisms/Error/NotFound";
import { PromptLayout } from "../../components/templates/Layout";

type Props = {};

const NotFoundPage = (props: Props) => {
  return (
    <PromptLayout>
      <NotFound
        icon="unlink"
        title="Không tìm thấy trang này"
        instruction="Liên kết bạn đang truy cập có thể bị gỡ hoặc không tồn tại"
        actionUrl="/"
        actionText="Quay về trang chủ"
      />
    </PromptLayout>
  );
};

export default NotFoundPage;
