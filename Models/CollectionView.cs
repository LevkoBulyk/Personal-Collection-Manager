using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Models
{
    public class CollectionView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Topic { get; set; }

        public string ImageUrl { get; set; }
    }
}
