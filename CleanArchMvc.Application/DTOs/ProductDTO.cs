using CleanArchMvc.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CleanArchMvc.Application.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name is Required")]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Descripton is Required")]
        [MinLength(5)]
        [MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The Price is Required")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "The Stock is Required")]
        [Range(1,9999)]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [MaxLength(250)]
        [Display(Name = "Product Image")]
        public string Image { get; set; }

        [Display(Name = "Categories")]
        public int CategoryId { get; set; }
        
        [JsonIgnore]
        public Category Category { get; set; }
    }
}
