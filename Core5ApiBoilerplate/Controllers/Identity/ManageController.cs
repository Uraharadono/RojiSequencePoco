//using System.Linq;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;
//using Core5ApiBoilerplate.DbContext.Entities.Identity;
//using Core5ApiBoilerplate.Infrastructure.Settings;
//using Core5ApiBoilerplate.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace Core5ApiBoilerplate.Controllers.Identity
//{
//    /*
//     * NOTE: We get user id from claims like so, and thus all of the "User" vars:
//     * https://stackoverflow.com/questions/46112258/how-do-i-get-current-user-in-net-core-web-api-from-jwt-token
//     */
//    [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ManageController : ControllerBase
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly RoleManager<ApplicationRole> _roleManager;
//        private readonly UrlEncoder _urlEncoder;
//        private readonly IEmailService _emailService;
//        private readonly IClientAppSettings _clientAppSettings;
//        private readonly IQRCodeSettings _qr;

//        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

//        public ManageController(
//            UserManager<ApplicationUser> userManager,
//            RoleManager<ApplicationRole> roleManager,
//            UrlEncoder urlEncoder,
//            IEmailService emailService,
//            // IOptions<IClientAppSettings> client,
//            IClientAppSettings client,
//            // IOptions<IQRCodeSettings> qr
//            IQRCodeSettings qr
//            )
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _urlEncoder = urlEncoder;
//            _emailService = emailService;
//            // _clientAppSettings = client.Value;
//            _clientAppSettings = client;
//            // _qr = qr.Value;
//            _qr = qr;
//        }

//        [HttpGet]
//        [Route("UserInfo")]
//        public async Task<IActionResult> UserInfo()
//        {
//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);

//            var userModel = new UserModel
//            {
//                Email = user.Email,
//                EmailConfirmed = user.EmailConfirmed,
//                LockoutEnabled = user.LockoutEnabled,
//                Roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false)
//            };

//            return Ok(userModel);
//        }

//        [HttpGet("TwoFactorAuthentication")]
//        public async Task<IActionResult> TwoFactorAuthentication()
//        {
//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            var model = new TwoFactorAuthenticationViewModel
//            {
//                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false) != null,
//                Is2faEnabled = user.TwoFactorEnabled,
//                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user).ConfigureAwait(false)
//            };

//            return Ok(model);
//        }

//        [HttpGet("EnableAuthenticator")]
//        public async Task<IActionResult> EnableAuthenticator()
//        {
//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            var model = new EnableAuthenticatorViewModel();
//            await LoadSharedKeyAndQrCodeUriAsync(user, model).ConfigureAwait(false);

//            return Ok(model);
//        }

//        [HttpPost("ChangePassword")]
//        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            var passwordValidator = new PasswordValidator<ApplicationUser>();
//            var result = await passwordValidator.ValidateAsync(_userManager, null, model.NewPassword).ConfigureAwait(false);

//            if (result.Succeeded)
//            {
//                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword).ConfigureAwait(false);
//                if (!changePasswordResult.Succeeded)
//                    return BadRequest(new string[] { "Could not change password!" });

//                return Ok(changePasswordResult);
//            }
//            else
//            {
//                return BadRequest(result.Errors.Select(x => x.Description));
//            }
//        }

//        [HttpPost("SendVerificationEmail")]
//        public async Task<IActionResult> SendVerificationEmail()
//        {
//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

//            var callbackUrl = $"{_clientAppSettings.ClientBaseUrl}{_clientAppSettings.EmailConfirmationPath}?uid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";
//            await _emailService.SendEmailConfirmationAsync(user.Email, callbackUrl).ConfigureAwait(false);

//            return Ok();
//        }

//        [HttpPost("SetPassword")]
//        public async Task<IActionResult> SetPassword([FromBody] SetPasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword).ConfigureAwait(false);

//            if (addPasswordResult.Succeeded)
//                return Ok(addPasswordResult);

//            return BadRequest(addPasswordResult.Errors.Select(x => x.Description));
//        }

//        [HttpPost("DisableTfa")]
//        public async Task<IActionResult> Disable2fa()
//        {
//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false).ConfigureAwait(false);
//            if (!disable2faResult.Succeeded)
//                return BadRequest(disable2faResult.Errors.Select(x => x.Description));

//            return Ok(disable2faResult);
//        }

//        [HttpPost("EnableAuthenticator")]
//        public async Task<IActionResult> EnableAuthenticator([FromBody] EnableAuthenticatorViewModel model)
//        {
//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            if (!ModelState.IsValid)
//            {
//                await LoadSharedKeyAndQrCodeUriAsync(user, model).ConfigureAwait(false);
//                return Ok(model);
//            }

//            // Strip spaces and hypens
//            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

//            var is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
//                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode).ConfigureAwait(false);

//            if (!is2FaTokenValid)
//            {
//                ModelState.AddModelError("Code", "Verification code is invalid.");
//                await LoadSharedKeyAndQrCodeUriAsync(user, model).ConfigureAwait(false);
//                // return View(model); // TODO: Wtf is this?
//            }

//            await _userManager.SetTwoFactorEnabledAsync(user, true).ConfigureAwait(false);
//            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10).ConfigureAwait(false);

//            return Ok(recoveryCodes.ToArray());
//        }

//        //[HttpGet]
//        //public IActionResult ShowRecoveryCodes()
//        //{
//        //    var recoveryCodes = (string[])TempData[RecoveryCodesKey];
//        //    if (recoveryCodes == null)
//        //    {
//        //        return RedirectToAction(nameof(TwoFactorAuthentication));
//        //    }

//        //    var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
//        //    return View(model);
//        //}

//        [HttpPost("ResetAuthenticator")]
//        public async Task<IActionResult> ResetAuthenticator()
//        {
//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            await _userManager.SetTwoFactorEnabledAsync(user, false).ConfigureAwait(false);
//            await _userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);

//            return Ok();
//        }

//        [HttpPost("GenerateRecoveryCodes")]
//        public async Task<IActionResult> GenerateRecoveryCodes()
//        {
//            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
//            if (user == null)
//                return BadRequest(new string[] { "Could not find user!" });

//            if (!user.TwoFactorEnabled)
//                return BadRequest(new string[] { $"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled." });

//            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10).ConfigureAwait(false);

//            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

//            return Ok(model);
//        }

//        private string FormatKey(string unformattedKey)
//        {
//            var result = new StringBuilder();
//            int currentPosition = 0;
//            while (currentPosition + 4 < unformattedKey.Length)
//            {
//                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
//                currentPosition += 4;
//            }
//            if (currentPosition < unformattedKey.Length)
//            {
//                result.Append(unformattedKey.Substring(currentPosition));
//            }

//            return result.ToString().ToLowerInvariant();
//        }

//        private string GenerateQrCodeUri(string email, string unformattedKey)
//        {
//            return string.Format(
//                AuthenticatorUriFormat,
//                _urlEncoder.Encode(_qr.QRCodeAppName),
//                _urlEncoder.Encode(email),
//                unformattedKey);
//        }

//        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
//        {
//            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
//            if (string.IsNullOrEmpty(unformattedKey))
//            {
//                await _userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
//                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
//            }

//            model.SharedKey = FormatKey(unformattedKey);
//            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
//        }
//    }
//}
