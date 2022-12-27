const farmCreationModel = {
  id: {
    value: 0,
    isValid: true,
  },
  name: {
    value: "",
    validation: {
      isRequired: true,
    },
    isValid: false,
  },
  description: {
    value: "",
    validation: {
      isRequired: true,
    },
    isValid: false,
  },
  address: {
    value: "",
    validation: {
      isRequired: false,
    },
    isValid: true,
  },
  farmTypeName: {
    value: "",
    isValid: true,
  },
  farmTypeId: {
    value: 0,
    validation: {
      isRequired: true,
    },
    isValid: false,
  },
  files: {
    value: [],
    isValid: true,
  },
};

export default farmCreationModel;
