using System.ComponentModel.DataAnnotations;

namespace CloudTagsApi.Models
{
    public class WordEntities
    {
        [Key]
        public string Id { get; set; }
        public string Word { get; set; }
        public int Count { get; set; }
    }
}
