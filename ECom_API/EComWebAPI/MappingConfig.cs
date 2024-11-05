using AutoMapper;
using EComWebAPI.Models;
using EComWebAPI.Models.Dto;

namespace EComWebAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();

            CreateMap<Category, CategoryCreateDTO>().ReverseMap();
            CreateMap<Category, CategoryUpdateDTO>().ReverseMap();

            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, ProductCreateDTO>().ReverseMap();
            CreateMap<Product, ProductUpdateDTO>().ReverseMap();
        }
    }
}
