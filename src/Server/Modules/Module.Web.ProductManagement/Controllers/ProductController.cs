﻿using Camino.Shared.Requests.Filters;
using Camino.Core.Constants;
using Camino.Shared.Enums;
using Camino.Framework.Attributes;
using Camino.Framework.Controllers;
using Camino.Core.Contracts.Helpers;
using Camino.Framework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Module.Web.ProductManagement.Models;
using System;
using System.Threading.Tasks;
using Camino.Core.Contracts.Services.Products;
using System.Linq;
using Camino.Shared.Requests.Products;
using Camino.Shared.Requests.Media;
using Camino.Core.Utils;
using System.Collections.Generic;

namespace Module.Web.ProductManagement.Controllers
{
    public class ProductController : BaseAuthController
    {
        private readonly IProductService _productService;
        private readonly IHttpHelper _httpHelper;

        public ProductController(IProductService productService, IHttpHelper httpHelper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _httpHelper = httpHelper;
            _productService = productService;
        }

        [ApplicationAuthorize(AuthorizePolicyConst.CanReadProduct)]
        [LoadResultAuthorizations("Product", PolicyMethod.CanCreate, PolicyMethod.CanUpdate, PolicyMethod.CanDelete)]
        public async Task<IActionResult> Index(ProductFilterModel filter)
        {
            var productPageList = await _productService.GetAsync(new ProductFilter
            {
                CreatedById = filter.CreatedById,
                CreatedDateFrom = filter.CreatedDateFrom,
                CreatedDateTo = filter.CreatedDateTo,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Search = filter.Search,
                UpdatedById = filter.UpdatedById,
                CategoryId = filter.CategoryId,
                StatusId = filter.StatusId,
                CanGetDeleted = true,
                CanGetInactived = true
            });
            var products = productPageList.Collections.Select(x => new ProductModel
            {
                Description = x.Description,
                CreatedBy = x.CreatedBy,
                CreatedById = x.CreatedById,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdateById = x.UpdatedById,
                UpdatedDate = x.UpdatedDate,
                Id = x.Id,
                Name = x.Name,
                StatusId = (ProductStatus)x.StatusId,
                PictureId = x.Pictures.Any() ? x.Pictures.FirstOrDefault().Id : 0
            });

            var productPage = new PageListModel<ProductModel>(products)
            {
                Filter = filter,
                TotalPage = productPageList.TotalPage,
                TotalResult = productPageList.TotalResult
            };

            if (_httpHelper.IsAjaxRequest(Request))
            {
                return PartialView("Partial/_ProductTable", productPage);
            }

            return View(productPage);
        }

        [ApplicationAuthorize(AuthorizePolicyConst.CanReadProduct)]
        [LoadResultAuthorizations("Product", PolicyMethod.CanUpdate)]
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
            {
                return RedirectToNotFoundPage();
            }

            try
            {
                var product = await _productService.FindDetailAsync(new IdRequestFilter<long>
                {
                    Id = id,
                    CanGetDeleted = true,
                    CanGetInactived = true
                });
                if (product == null)
                {
                    return RedirectToNotFoundPage();
                }

                var model = new ProductModel()
                {
                    Id = product.Id,
                    UpdateById = product.UpdatedById,
                    UpdatedBy = product.UpdatedBy,
                    CreatedBy = product.CreatedBy,
                    CreatedById = product.CreatedById,
                    CreatedDate = product.CreatedDate,
                    UpdatedDate = product.UpdatedDate,
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    StatusId = (ProductStatus)product.StatusId,
                    Pictures = product.Pictures.Select(y => new PictureRequestModel()
                    {
                        PictureId = y.Id
                    }),
                    ProductCategories = product.Categories.Select(x => new ProductCategoryRelationModel()
                    {
                        Id = x.Id,
                        Name = x.Name
                    }),
                    ProductFarms = product.Farms.Select(x => new ProductFarmModel()
                    {
                        FarmId = x.FarmId,
                        FarmName = x.Name
                    }),
                    ProductAttributes = product.ProductAttributes.Select(x => new ProductAttributeRelationModel
                    {
                        AttributeId = x.AttributeId,
                        ControlTypeId = x.AttributeControlTypeId,
                        DisplayOrder = x.DisplayOrder,
                        Id = x.Id,
                        IsRequired = x.IsRequired,
                        TextPrompt = x.TextPrompt,
                        Name = x.AttributeName,
                        ControlTypeName = ((ProductAttributeControlType)x.AttributeControlTypeId).GetEnumDescription(),
                        AttributeRelationValues = x.AttributeRelationValues.Select(c => new ProductAttributeRelationValueModel
                        {
                            Id = c.Id,
                            DisplayOrder = c.DisplayOrder,
                            Name = c.Name,
                            PriceAdjustment = c.PriceAdjustment,
                            PricePercentageAdjustment = c.PricePercentageAdjustment,
                            Quantity = c.Quantity
                        })
                    })
                };
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToErrorPage();
            }
        }

        [HttpGet]
        [ApplicationAuthorize(AuthorizePolicyConst.CanUpdateProduct)]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productService.FindDetailAsync(new IdRequestFilter<long>
            {
                Id = id,
                CanGetDeleted = true,
                CanGetInactived = true
            });
            if (product == null)
            {
                return RedirectToNotFoundPage();
            }
            var model = new ProductModel()
            {
                Id = product.Id,
                UpdateById = product.UpdatedById,
                UpdatedBy = product.UpdatedBy,
                CreatedBy = product.CreatedBy,
                CreatedById = product.CreatedById,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                StatusId = (ProductStatus)product.StatusId,
                Pictures = product.Pictures.Select(y => new PictureRequestModel()
                {
                    PictureId = y.Id
                }),
                ProductCategories = product.Categories.Select(x => new ProductCategoryRelationModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }),
                ProductFarms = product.Farms.Select(x => new ProductFarmModel()
                {
                    FarmId = x.FarmId,
                    FarmName = x.Name
                }),
                ProductAttributes = product.ProductAttributes.Select(x => new ProductAttributeRelationModel
                {
                    AttributeId = x.AttributeId,
                    ControlTypeId = x.AttributeControlTypeId,
                    DisplayOrder = x.DisplayOrder,
                    Id = x.Id,
                    IsRequired = x.IsRequired,
                    TextPrompt = x.TextPrompt,
                    Name = x.AttributeName,
                    ControlTypeName = ((ProductAttributeControlType)x.AttributeControlTypeId).GetEnumDescription(),
                    AttributeRelationValues = x.AttributeRelationValues.Select(c => new ProductAttributeRelationValueModel
                    {
                        Id = c.Id,
                        DisplayOrder = c.DisplayOrder,
                        Name = c.Name,
                        PriceAdjustment = c.PriceAdjustment,
                        PricePercentageAdjustment = c.PricePercentageAdjustment,
                        Quantity = c.Quantity
                    })
                })
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicyConst.CanUpdateProduct)]
        public async Task<IActionResult> Update(ProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var product = new ProductModifyRequest()
            {
                Id = model.Id,
                UpdatedById = LoggedUserId,
                CreatedById = LoggedUserId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Categories = model.ProductCategoryIds.Select(id => new ProductCategoryRequest()
                {
                    Id = id
                }),
                Farms = model.ProductFarmIds.Select(id => new ProductFarmRequest()
                {
                    FarmId = id
                }),
                ProductAttributes = model.ProductAttributes?.Select(x => new ProductAttributeRelationRequest
                {
                    Id = x.Id,
                    ControlTypeId = x.ControlTypeId,
                    DisplayOrder = x.DisplayOrder,
                    ProductAttributeId = x.AttributeId,
                    AttributeRelationValues = x.AttributeRelationValues?.Select(c => new ProductAttributeRelationValueRequest
                    {
                        Id = c.Id,
                        DisplayOrder = c.DisplayOrder,
                        Name = c.Name,
                        PriceAdjustment = c.PriceAdjustment,
                        PricePercentageAdjustment = c.PricePercentageAdjustment,
                        Quantity = c.Quantity
                    })
                })
            };
            if (product.Id <= 0)
            {
                return RedirectToErrorPage();
            }

            var exist = await _productService.FindAsync(new IdRequestFilter<long>
            {
                Id = model.Id,
                CanGetDeleted = true,
                CanGetInactived = true
            });
            if (exist == null)
            {
                return RedirectToErrorPage();
            }

            if (model.Pictures != null && model.Pictures.Any())
            {
                product.Pictures = model.Pictures.Select(x => new PictureRequest()
                {
                    Base64Data = x.Base64Data,
                    ContentType = x.ContentType,
                    FileName = x.FileName,
                    Id = x.PictureId
                });
            }

            product.UpdatedById = LoggedUserId;
            await _productService.UpdateAsync(product);
            return RedirectToAction(nameof(Detail), new { id = product.Id });
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicyConst.CanDeleteProduct)]
        public async Task<IActionResult> Delete(ProductIdRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var isDeleted = await _productService.DeleteAsync(request.Id);
            if (!isDeleted)
            {
                return RedirectToErrorPage();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicyConst.CanUpdateProduct)]
        public async Task<IActionResult> TemporaryDelete(ProductIdRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var isDeleted = await _productService.SoftDeleteAsync(new ProductModifyRequest
            {
                Id = request.Id,
                UpdatedById = LoggedUserId
            });
            if (!isDeleted)
            {
                return RedirectToErrorPage();
            }

            if (request.ShouldKeepUpdatePage)
            {
                return RedirectToAction(nameof(Update), new { id = request.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicyConst.CanUpdateProduct)]
        public async Task<IActionResult> Deactivate(ProductIdRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var isInactived = await _productService.DeactiveAsync(new ProductModifyRequest
            {
                Id = request.Id,
                UpdatedById = LoggedUserId
            });

            if (!isInactived)
            {
                return RedirectToErrorPage();
            }

            if (request.ShouldKeepUpdatePage)
            {
                return RedirectToAction(nameof(Update), new { id = request.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicyConst.CanUpdateProduct)]
        public async Task<IActionResult> Active(ProductIdRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToErrorPage();
            }

            var isActived = await _productService.ActiveAsync(new ProductModifyRequest
            {
                Id = request.Id,
                UpdatedById = LoggedUserId
            });

            if (!isActived)
            {
                return RedirectToErrorPage();
            }

            if (request.ShouldKeepUpdatePage)
            {
                return RedirectToAction(nameof(Update), new { id = request.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        [ApplicationAuthorize(AuthorizePolicyConst.CanReadPicture)]
        [LoadResultAuthorizations("Picture", PolicyMethod.CanCreate, PolicyMethod.CanUpdate, PolicyMethod.CanDelete)]
        public async Task<IActionResult> Pictures(ProductPictureFilterModel filter)
        {
            var filterRequest = new ProductPictureFilter()
            {
                CreatedById = filter.CreatedById,
                CreatedDateFrom = filter.CreatedDateFrom,
                CreatedDateTo = filter.CreatedDateTo,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Search = filter.Search,
                MimeType = filter.MimeType
            };

            var productPicturePageList = await _productService.GetPicturesAsync(filterRequest);
            var productPictures = productPicturePageList.Collections.Select(x => new ProductPictureModel
            {
                ProductName = x.ProductName,
                ProductId = x.ProductId,
                PictureId = x.PictureId,
                PictureName = x.PictureName,
                PictureCreatedBy = x.PictureCreatedBy,
                PictureCreatedById = x.PictureCreatedById,
                PictureCreatedDate = x.PictureCreatedDate,
                ProductPictureType = (ProductPictureType)x.ProductPictureTypeId,
                ContentType = x.ContentType
            });

            var productPage = new PageListModel<ProductPictureModel>(productPictures)
            {
                Filter = filter,
                TotalPage = productPicturePageList.TotalPage,
                TotalResult = productPicturePageList.TotalResult
            };

            if (_httpHelper.IsAjaxRequest(Request))
            {
                return PartialView("Partial/_ProductPictureTable", productPage);
            }

            return View(productPage);
        }

        [HttpGet]
        [ApplicationAuthorize(AuthorizePolicyConst.CanReadProduct)]
        public IActionResult SearchStatus(string q, int? currentId = null)
        {
            var statuses = _productService.SearchStatus(new IdRequestFilter<int?>
            {
                Id = currentId
            }, q);

            if (statuses == null || !statuses.Any())
            {
                return Json(new List<Select2ItemModel>());
            }

            var categorySeletions = statuses
                .Select(x => new Select2ItemModel
                {
                    Id = x.Id.ToString(),
                    Text = x.Text
                });

            return Json(categorySeletions);
        }

        [HttpGet]
        [ApplicationAuthorize(AuthorizePolicyConst.CanReadProductAttribute)]
        public async Task<IActionResult> GetAttributeRelation(long id)
        {
            var attribute = await _productService.GetAttributeRelationByIdAsync(id);
            if (attribute == null)
            {
                return NotFound();
            }

            return Json(new ProductAttributeRelationModel
            {
                AttributeId = attribute.AttributeId,
                Id = attribute.Id,
                ControlTypeId = attribute.AttributeControlTypeId,
                ControlTypeName = ((ProductAttributeControlType)attribute.AttributeControlTypeId).GetEnumDescription(),
                DisplayOrder = attribute.DisplayOrder,
                IsRequired = attribute.IsRequired,
                Name = attribute.AttributeName,
                TextPrompt = attribute.TextPrompt
            });
        }

        [HttpPost]
        [ApplicationAuthorize(AuthorizePolicyConst.CanUpdateProduct)]
        public IActionResult AddAttributeRelation(ProductAttributeRelationModel request)
        {
            return PartialView("Partial/_ProductAttributeUpdate", request);
        }
    }
}