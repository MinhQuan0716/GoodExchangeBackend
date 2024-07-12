using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.SubscriptionHistoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class SubscriptionHistoryService : ISubscriptionHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubscriptionHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<SubscriptionHistoryViewModel>> GetAllSubscriptionHistoriesAsync()
        {
            return await _unitOfWork.SubscriptionHistoryRepository.GetAllSubscriptionHistory();
        }
    }
}
