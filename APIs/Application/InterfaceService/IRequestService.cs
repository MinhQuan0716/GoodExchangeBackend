using Application.ViewModel.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IRequestService
    {
        Task<bool>SendRequest(CreateRequestModel requestModel);
        Task<List<RequestViewModel>> GetAllRequestsOfCurrentUserAsync();
    }
}
