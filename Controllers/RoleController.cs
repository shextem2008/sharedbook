using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SHB.Business.Services;
using SHB.Core.Domain.DataTransferObjects;
using SHB.WebApi.Utils;

namespace SHB.WebApi.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleSvc;

        public RoleController(IRoleService roleSvc)
        {
            _roleSvc = roleSvc;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IServiceResponse<bool>> Create(RoleDTO role)
        {
            return await HandleApiOperationAsync(async () => {
                var result = await _roleSvc.CreateAsync(role);

                return new ServiceResponse<bool>(result);
            });
        }

        [HttpPost]
        [Route("Adds")]
        public async Task<IServiceResponse<bool>> Create(RolesDTO role)
        {
            return await HandleApiOperationAsync(async () => {
                var result = await _roleSvc.CreateAsync(role);

                return new ServiceResponse<bool>(result);
            });
        }

        [HttpGet]
        [Route("Get")]
        [Route("Get/{pageNumber}/{pageSize}")]
        [Route("Get/{pageNumber}/{pageSize}/{query}")]
        public async Task<ServiceResponse<IPagedList<RoleDTO>>> GetRoutes(
            int pageNumber = 1,
            int pageSize = WebConstants.DefaultPageSize,
            string query = null)
        {
            return await HandleApiOperationAsync(async () => {

                var roles = await _roleSvc.Get(pageNumber, pageSize, query);

                return new ServiceResponse<IPagedList<RoleDTO>>
                {
                    Object = roles
                };
            });
        }

        [HttpGet]
        [Route("claimget")]
        [Route("claimget/{pageNumber}/{pageSize}")]
        [Route("claimget/{pageNumber}/{pageSize}/{query}")]
        public async Task<ServiceResponse<IPagedList<ClaimDTO>>> GetClaims(
            int pageNumber = 1,
            int pageSize = WebConstants.DefaultPageSize,
            string query = null)
        {
            return await HandleApiOperationAsync(async () => {

                var roles = await _roleSvc.Get(pageNumber, pageSize, query);

                return new ServiceResponse<IPagedList<ClaimDTO>>
                {
                    Object = (IPagedList<ClaimDTO>)roles
                };
            });
        }

        //[HttpGet]
        //[Route("claimget")]
        //[Route("claimget/{pageNumber}/{pageSize}")]
        //[Route("claimget/{pageNumber}/{pageSize}/{query}")]
        //public async Task<ServiceResponse<IPagedList<Claim>>> GetClaims(
        //    int pageNumber = 1,
        //    int pageSize = WebConstants.DefaultPageSize,
        //    string query = null)
        //{
        //    return await HandleApiOperationAsync(async () => {

        //        var roles = await _roleSvc.Get(pageNumber, pageSize, query);

        //        return new ServiceResponse<IPagedList<Claim>>
        //        {
        //            Object = (IPagedList<Claim>)roles
        //        };
        //    });
        //}

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IServiceResponse<bool>> Delete(int id)
        {
            return await HandleApiOperationAsync(async () => {
                await _roleSvc.RemoveRole(id);

                return new ServiceResponse<bool>(true);
            });
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IServiceResponse<bool>> Update(int id, RolesDTO role)
        {
            return await HandleApiOperationAsync(async () => {
                await _roleSvc.UpdateRole(id, role);

                return new ServiceResponse<bool>(true);
            });
        }


        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IServiceResponse<RolesDTO>> GetRoleById(int id)
        {
            return await HandleApiOperationAsync(async () => {
                var info = await _roleSvc.GetRoleById(id);

                return new ServiceResponse<RolesDTO>
                {
                    Object = info
                };
            });
        }

        //[HttpGet]
        //[Route("getclaims")]
        //public IActionResult GetClaims()
        //{
        //    var claims = 

        //    return null;
        //}
    }
}
