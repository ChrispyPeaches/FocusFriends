using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers
{
    // Object returned from calls to the BadgeService
    internal class BadgeEligibilityResult
    {
        public bool IsEligible { get; set; } = false;
        public Badge? EarnedBadge { get; set; } = null;
    }
}
