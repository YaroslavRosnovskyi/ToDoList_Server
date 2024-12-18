﻿using AutoMapper;
using ToDoList.BLL.DTO.ToDo;
using ToDoList.DAL.Entities;

namespace ToDoList.BLL.Mapping.ToDos;

public class ToDoProfile : Profile
{
    public ToDoProfile()
    {
        CreateMap<ToDo, ToDoDTO>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("O")))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate.ToString("O")))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status!.Name))
            .ReverseMap();
        CreateMap<ToDoUpdateDTO, ToDo>().ReverseMap();
        CreateMap<ToDoCreateDTO, ToDo>();
    }
}