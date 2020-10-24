using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SHB.Data.efCore.Context;

namespace SHB.WebApi.Controllers
{
    [Authorize]
    public class ClaimController : BaseController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _context;

        public ClaimController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
                ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    var claims = from x in _context.Roles
        //                 join b in _context.RoleClaims on x.Id equals b
        //}


        //private readonly IStateService _stateService;

        //public ClaimController(IStateService stateService)
        //{
        //    _stateService = stateService;
        //}

        //[HttpGet]
        //[Route("Get")]
        //[Route("Get/{pageNumber}/{pageSize}")]
        //[Route("Get/{pageNumber}/{pageSize}/{query}")]
        //public async Task<IServiceResponse<IPagedList<StateDTO>>> GetStates(int pageNumber = 1,
        //    int pageSize = WebConstants.DefaultPageSize, string query = null)
        //{
        //    return await HandleApiOperationAsync(async () => {
        //        var states = await _stateService.GetStates(pageNumber, pageSize);

        //        return new ServiceResponse<IPagedList<StateDTO>>
        //        {
        //            Object = states
        //        };
        //    });
        //}

        //[HttpGet]
        //[Route("Get/{id}")]
        //public async Task<IServiceResponse<StateDTO>> GetStateById(int id)
        //{
        //    return await HandleApiOperationAsync(async () => {
        //        var state = await _stateService.GetStateById(id);

        //        return new ServiceResponse<StateDTO>
        //        {
        //            Object = state
        //        };
        //    });
        //}

        //[HttpPost]
        //[Route("Add")]
        //public async Task<IServiceResponse<bool>> AddState(StateDTO state)
        //{
        //    return await HandleApiOperationAsync(async () => {
        //        await _stateService.AddState(state);

        //        return new ServiceResponse<bool>(true);
        //    });
        //}

        //[HttpPut]
        //[Route("Update/{id}")]
        //public async Task<IServiceResponse<bool>> UpdateState(int id, StateDTO state)
        //{
        //    return await HandleApiOperationAsync(async () => {
        //        await _stateService.UpdateState(id, state);

        //        return new ServiceResponse<bool>(true);
        //    });
        //}

        //[HttpDelete]
        //[Route("Delete/{id}")]
        //public async Task<IServiceResponse<bool>> DeleteState(int id)
        //{
        //    return await HandleApiOperationAsync(async () => {
        //        await _stateService.RemoveState(id);

        //        return new ServiceResponse<bool>(true);
        //    });
        //}
    }
}
