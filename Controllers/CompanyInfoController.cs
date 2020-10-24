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

    public class CompanyInfoController : BaseController
    {
        private readonly ICompanyInfo _companyInfo;

        public CompanyInfoController(ICompanyInfo companyInfo)
        {
            _companyInfo = companyInfo;
        }


        [HttpPost]
        [Route("Add")]
        public async Task<IServiceResponse<bool>> AddCompanyInfo(CompanyInfoDTO companyInfo)
        {
            return await HandleApiOperationAsync(async () => {
                await _companyInfo.AddCompanyInfo(companyInfo);

                return new ServiceResponse<bool>(true);

            });
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IServiceResponse<bool>> UpdateCompanyInfo(int id, CompanyInfoDTO companyInfo)
        {
            return await HandleApiOperationAsync(async () => {
                await _companyInfo.UpdateCompanyInfo(id, companyInfo);

                return new ServiceResponse<bool>(true);
            });
        }


        [HttpGet]
        [Route("Get")]
        [Route("Get/{pageNumber}/{pageSize}")]
        [Route("Get/{pageNumber}/{pageSize}/{query}")]
        public async Task<IServiceResponse<IPagedList<CompanyInfoDTO>>> GetcompanyInfo(int pageNumber = 1, int pageSize = WebConstants.DefaultPageSize, string query = null)
        {
            return await HandleApiOperationAsync(async () => {
                var info = await _companyInfo.GetcompanyInfo(pageNumber, pageSize, query);

                return new ServiceResponse<IPagedList<CompanyInfoDTO>>
                {
                    Object = info
                };
            });
        }

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IServiceResponse<CompanyInfoDTO>> GetcompanyInfoById(int id)
        {
            return await HandleApiOperationAsync(async () => {
                var info = await _companyInfo.GetcompanyInfoById(id);

                return new ServiceResponse<CompanyInfoDTO>
                {
                    Object = info
                };
            });
        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IServiceResponse<bool>> DeleteCompanyInfo(int id)
        {
            return await HandleApiOperationAsync(async () => {
                await _companyInfo.RemoveCompanyInfo(id);

                return new ServiceResponse<bool>(true);
            });
        }
    }
}
