﻿using Module.Web.SetupManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Camino.Core.Domain.Identities;
using Camino.Core.Contracts.IdentityManager;
using Camino.Shared.Enums;
using Camino.Shared.Requests.Setup;
using Camino.Core.Contracts.Providers;
using Camino.Core.Contracts.Services.Setup;
using Camino.Shared.Requests.Identifiers;

namespace Module.Web.SetupManagement.Controllers
{
    public class SetupController : Controller
    {
        private readonly IDataSeedService _dataSeedService;
        private readonly ISetupProvider _setupProvider;
        private readonly IUserSecurityStampStore<ApplicationUser> _userSecurityStampStore;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IUserManager<ApplicationUser> _userManager;
        private readonly ILoginManager<ApplicationUser> _loginManager;

        public SetupController(ISetupProvider setupProvider,
            IUserSecurityStampStore<ApplicationUser> userSecurityStampStore,
            IPasswordHasher<ApplicationUser> passwordHasher, IUserManager<ApplicationUser> userManager,
            IDataSeedService dataSeedService, ILoginManager<ApplicationUser> loginManager)
        {
            _setupProvider = setupProvider;
            _userSecurityStampStore = userSecurityStampStore;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
            _loginManager = loginManager;
            _dataSeedService = dataSeedService;
        }

        [HttpGet]
        public IActionResult StartSetup()
        {
            if (_setupProvider.HasDataSeeded())
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            if (_setupProvider.HasDatabaseSetup())
            {
                return RedirectToAction(nameof(SeedData));
            }

            return View(new StartSetupModel());
        }

        [HttpPost]
        public async Task<IActionResult> StartSetup(StartSetupModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_setupProvider.HasDataSeeded())
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            if (_setupProvider.HasDatabaseSetup())
            {
                return RedirectToAction(nameof(SeedData));
            }

            if (User != null && User.Identity.IsAuthenticated)
            {
                await _loginManager.SignOutAsync();
            }

            var settings = _setupProvider.LoadSettings();

            try
            {
                // Create database schema
                var contentDbScript = _setupProvider.LoadFileText(settings.CreateDatabaseScriptFilePath);
                await _dataSeedService.CreateDatabaseAsync(contentDbScript);

                _setupProvider.SetDatabaseHasBeenSetup();
                return RedirectToAction(nameof(SeedData));
            }
            catch (Exception)
            {
                _setupProvider.DeleteSetupSettings();
                return View();
            }
        }

        [HttpGet]
        public IActionResult SeedData()
        {
            if (_setupProvider.HasDataSeeded())
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            if (!_setupProvider.HasDatabaseSetup())
            {
                return RedirectToAction(nameof(StartSetup));
            }

            return View(new SetupModel());
        }

        [HttpPost]
        public async Task<IActionResult> SeedData(SetupModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_setupProvider.HasDataSeeded())
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            if (!_setupProvider.HasDatabaseSetup())
            {
                return RedirectToAction(nameof(StartSetup));
            }

            try
            {
                var settings = _setupProvider.LoadSettings();
                var initialUser = new ApplicationUser()
                {
                    DisplayName = $"{model.Lastname} {model.Firstname}",
                    Email = model.AdminEmail,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    StatusId = (byte)UserStatus.Actived,
                    UserName = model.AdminEmail,
                };

                initialUser.PasswordHash = _passwordHasher.HashPassword(initialUser, model.AdminPassword);
                await _userSecurityStampStore.SetSecurityStampAsync(initialUser, _userManager.NewSecurityStamp(), default);

                // Get initial data in josn
                var indentityJson = _setupProvider.LoadFileText(settings.SeedDataJsonFilePath);
                var setupRequest = JsonConvert.DeserializeObject<SetupRequest>(indentityJson);
                setupRequest.InitualUser = new UserModifyRequest
                {
                    BirthDate = initialUser.BirthDate,
                    Address = initialUser.Address,
                    CountryId = initialUser.CountryId,
                    Email = initialUser.Email,
                    Firstname = initialUser.Firstname,
                    Lastname = initialUser.Lastname,
                    PasswordHash = initialUser.PasswordHash,
                    SecurityStamp = initialUser.SecurityStamp,
                    UserName = initialUser.UserName,
                    DisplayName = initialUser.DisplayName,
                };

                // Initialize database
                await _dataSeedService.SeedDataAsync(setupRequest);

                _setupProvider.SetDataHasBeenSeeded();
                return RedirectToAction(nameof(Succeed));
            }
            catch (Exception)
            {
                _setupProvider.DeleteSetupSettings();
                return View();
            }
        }

        [HttpGet]
        public IActionResult Succeed()
        {
            return View();
        }
    }
}