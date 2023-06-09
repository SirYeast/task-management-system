﻿using TaskManagementSystem.Areas.Identity.Data;

namespace TaskManagementSystem.Models.ViewModels
{
    public class AssignTaskViewModel
    {
        public int TaskId { get; set; }
        public ApplicationTask Task { get; set; } = null!;

        public IEnumerable<ApplicationUser> Developers { get; set; } = null!;
        public string SelectedDeveloperId { get; set; } = null!;
    }
}
