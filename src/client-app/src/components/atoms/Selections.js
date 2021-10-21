import styled from "styled-components";

export const Selection = styled.select`
  height: ${(p) => p.theme.size.normal};
  border: 1px solid ${(p) => p.theme.color.neutralBg};
  padding: 6px 5px;
  border-radius: ${(p) => p.theme.borderRadius.normal};
  font-size: ${(p) => p.theme.fontSize.normal};
  :focus {
    outline: 0;
  }
`;

export const SelectionPrimary = styled(Selection)`
  border: 1px solid ${(p) => p.theme.color.primaryBg};
`;

export const SelectionSecondary = styled(Selection)`
  border: 1px solid ${(p) => p.theme.color.secondaryBg};
`;
