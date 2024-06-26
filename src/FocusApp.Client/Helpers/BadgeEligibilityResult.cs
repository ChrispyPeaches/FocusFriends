﻿using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers
{
    // Object returned from calls to the BadgeService
    internal class BadgeEligibilityResult
    {
        public bool IsEligible { get; set; } = false;
        public Badge? EarnedBadge { get; set; } = null;
    }
}
