using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Api.ViewModels
{
    public record GroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public string Mnemonic { get; set; }
    }
}
