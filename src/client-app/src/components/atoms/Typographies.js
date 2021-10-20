import styled from "styled-components";

const TypographySecondary = styled.p`
  color: ${(p) => p.theme.color.neutralText};
  font-size: ${(p) => p.theme.fontSize.small};

  a {
    color: inherit;
  }
`;

const TypographyDark = styled.p`
  color: ${(p) => p.theme.color.darkText};
  font-size: ${(p) => p.theme.fontSize.small};
`;

const TypographyTitle = styled.p`
  color: ${(p) => p.theme.color.secondaryTitle};
  font-size: ${(p) => p.theme.fontSize.small};
`;

export { TypographySecondary, TypographyDark, TypographyTitle };
