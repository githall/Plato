﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Files;
using PlatoCore.Stores.Abstractions.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{

    public class EditProfileViewProvider : ViewProviderBase<EditProfileViewModel>
    {

        private static string _pathToImages;
        private static string _urlToImages;
        
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly UserManager<User> _userManager;
        private readonly ISitesFolder _sitesFolder;

        public EditProfileViewProvider(
            IUserPhotoStore<UserPhoto> userPhotoStore,
            IPlatoUserStore<User> platoUserStore,
            IHostEnvironment hostEnvironment,
            IShellSettings shellSettings,
            UserManager<User> userManager,
            ISitesFolder sitesFolder,
            IFileStore fileStore)
        {
            _platoUserStore = platoUserStore;
            _userPhotoStore = userPhotoStore;
            _sitesFolder = sitesFolder;
            _userManager = userManager;
     
            // paths
            _pathToImages = fileStore.Combine(hostEnvironment.ContentRootPath, shellSettings.Location, "images");
            _urlToImages = $"/sites/{shellSettings.Location.ToLower()}/images/";
            
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(EditProfileViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(EditProfileViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(EditProfileViewModel viewModel, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, context);
            }
            
            return Views(
                View<User>("Home.Edit.Header", model => user).Zone("header"),
                View<User>("Home.Edit.Sidebar", model => user).Zone("content-left"),
                View<User>("Home.Edit.Tools", model => user).Zone("header-right"),
                View<EditProfileViewModel>("Home.EditProfile.Content", model => viewModel).Zone("content"),
                View<EditProfileViewModel>("Home.EditProfile.ContentRight", model => viewModel).Zone("content-right"),
                View<User>("Home.Edit.Footer", model => user).Zone("actions")
            );

        }

        public override async Task<bool> ValidateModelAsync(EditProfileViewModel viewModel, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditProfileViewModel()
            {
                DisplayName = viewModel.DisplayName,
                Location = viewModel.Location,
                Biography = viewModel.Biography,
                Url = viewModel.Url
            });
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(EditProfileViewModel userProfile, IViewProviderContext context)
        {
            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, context);
            }

            var model = new EditProfileViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(userProfile, context);
            }
            
            if (context.Updater.ModelState.IsValid)
            {

                // Update user avatar

                if (model.AvatarFile != null)
                {
                   user.PhotoUrl = await UpdateUserPhoto(user, model.AvatarFile);
                }

                await _userManager.UpdateAsync(user);
                
            }

            return await BuildEditAsync(userProfile, context);

        }

        #endregion

        #region "Private Methods"

        async Task<string> UpdateUserPhoto(User user, IFormFile file)
        {

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var stream = file.OpenReadStream();
            byte[] bytes = null;
            if (stream != null)
            {
                bytes = stream.ToByteArray();
            }

            // Ensure we have a valid byte array
            if (bytes == null)
            {
                return string.Empty;
            }
            
            // Get any existing photo
            var existingPhoto = await _userPhotoStore.GetByUserIdAsync(user.Id);

            // Upload the new file
            var fileName = await _sitesFolder.SaveUniqueFileAsync(stream, file.FileName, _pathToImages);
           
            // Ensure the new file was created
            if (!string.IsNullOrEmpty(fileName))
            {
                // Delete any existing file
                if (existingPhoto != null)
                {
                    //_sitesFolder.DeleteFile(existingPhoto.Name, _pathToImages);
                }
            }

            // Insert or update photo entry
            var id = existingPhoto?.Id ?? 0;
            var userPhoto = new UserPhoto
            {
                Id = id,
                UserId = user.Id,
                Name = fileName,
                ContentType = file.ContentType,
                ContentLength = file.Length,
                ContentBlob = bytes,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };
            
            var newOrUpdatedPhoto = id > 0
                ? await _userPhotoStore.UpdateAsync(userPhoto)
                : await _userPhotoStore.CreateAsync(userPhoto);
            if (newOrUpdatedPhoto != null)
            {
                return _urlToImages + newOrUpdatedPhoto.Name;
            }
            
            return string.Empty;

        }

        #endregion

    }

}
