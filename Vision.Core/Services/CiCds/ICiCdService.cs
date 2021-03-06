﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface ICiCdService
    {
        Task<CiCdDto> CreateAsync(CiCdDto dto);        
        Task<CiCdDto> GetByIdAsync(Guid cicdId);
        Task<CiCdDto> UpdateAsync(CiCdDto dto);
        Task<IPaginatedList<CiCdDto>> GetAsync(int pageIndex = 1, int pageSize = 10);
    }
}