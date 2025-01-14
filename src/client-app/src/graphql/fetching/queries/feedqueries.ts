import { gql } from "@apollo/client";

export const GET_USER_FEEDS = gql`
  query ($criterias: FeedFilterModelInput) {
    userFeeds(criterias: $criterias) {
      totalPage
      totalResult
      filter {
        page
        pageSize
        search
      }
      collections {
        id
        description
        name
        createdDate
        createdByIdentityId
        address
        feedType
        createdByName
        pictureId
        price
        createdByPhotoId
      }
    }
  }
`;

export const GET_FEEDS = gql`
  query ($criterias: FeedFilterModelInput) {
    feeds(criterias: $criterias) {
      totalPage
      totalResult
      filter {
        page
        pageSize
        search
      }
      collections {
        id
        description
        name
        createdDate
        createdByIdentityId
        address
        feedType
        createdByName
        pictureId
        price
        createdByPhotoId
      }
    }
  }
`;

export const ADVANCED_SEARCH = gql`
  query ($criterias: FeedFilterModelInput) {
    advancedSearch(criterias: $criterias) {
      articles {
        id
        description
        name
        createdDate
        createdByIdentityId
        address
        feedType
        createdByName
        pictureId
        price
        createdByPhotoId
      }
      totalArticle
      totalArticlePage
      products {
        id
        description
        name
        createdDate
        createdByIdentityId
        address
        feedType
        createdByName
        pictureId
        price
        createdByPhotoId
      }
      totalProduct
      totalProductPage
      farms {
        id
        description
        name
        createdDate
        createdByIdentityId
        address
        feedType
        createdByName
        pictureId
        price
        createdByPhotoId
      }
      totalFarm
      totalFarmPage
      users {
        id
        description
        name
        createdDate
        createdByIdentityId
        address
        feedType
        createdByName
        pictureId
        price
        createdByPhotoId
      }
      totalUser
      totalUserPage
      userFilterByName
      page
    }
  }
`;

export const LIVE_SEARCH = gql`
  query ($criterias: FeedFilterModelInput) {
    liveSearch(criterias: $criterias) {
      articles {
        id
        name
        feedType
        pictureId
      }
      products {
        id
        name
        feedType
        pictureId
      }
      farms {
        id
        name
        feedType
        pictureId
      }
      users {
        id
        name
        feedType
        pictureId
      }
    }
  }
`;
