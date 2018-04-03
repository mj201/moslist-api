using Microsoft.AspNetCore.Mvc;
using MosListAPI.DataAccess;
using MosListAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MosListAPI.Controllers {
    [Route("ad")]
    public class AdController : Controller {
        #region " Members "
        IRepository<Ad> _adRepository;
        #endregion
        #region " Constructors "
        public AdController(IRepository<Ad> adStore){
            _adRepository = adStore;
        }
        #endregion
        [HttpGet("/ad")]
        public async Task<IActionResult> GetAsync(){
            var ads = await _adRepository.GetAllAsync();
            return new ObjectResult(ads){
                StatusCode = 200
            };
        }

        // TODO : standardize error objects
        [HttpGet("/ad/{id}")]
        public async Task<IActionResult> GetAdAsync(string id){
            var ad = await _adRepository.GetAsync(id);
            if (ad == null || ad == default(Ad))
                return new NotFoundObjectResult(null);
            return new ObjectResult(ad){
                StatusCode = 200
            };
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]AdPostRequest adRequest){
            if (!ModelState.IsValid){
                return new BadRequestObjectResult(ModelState);
            }
            return await Task.FromResult(
                new ObjectResult(await _adRepository.CreateAsync(new Ad(adRequest)))
                { 
                    StatusCode = 200 
                }
            );
        }
    }
}
 