using System;
using AutoMapper;
using IceSync.Services.Models;
using IceSync.Models;
using IceSync.DataCore.Models;

namespace IceSync.Automapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<WorkflowDTO, WorkflowOutputModel>()
            .ReverseMap();

        CreateMap<WorkflowDTO, Workflow>()
            .ReverseMap();
    }
}
