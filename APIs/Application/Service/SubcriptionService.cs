using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.SubcriptionModel;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class SubcriptionService : ISubcriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SubcriptionService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateSubcription(CreateSubcriptionModel createSubcriptionModel)
        {
            var subcription = _mapper.Map<Subcription>(createSubcriptionModel);
            await _unitOfWork.SubcriptionRepository.AddAsync(subcription);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
