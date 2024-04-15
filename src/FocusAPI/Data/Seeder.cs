using FocusAPI.Models;
using FocusCore.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace FocusAPI.Data
{
    public class Seeder
    {
        private readonly FocusAPIContext _apiContext;
        
        public Seeder(FocusAPIContext apiContext)
        {
            _apiContext = apiContext;
        }

        private List<MindfulnessTip> getMindfulnessTips()
        {
            return new()
            {
                new MindfulnessTip
                {
                    Id = Guid.NewGuid(),
                    Title = $"4-4-4 Breathing Exercise",
                    SessionRatingLevel = ((MindfulnessTipExtensions.FocusSessionRating.Good)).ToString(),
                    Content = """
                              <html>
                                  </body>
                                      <div style="display: flex;">
                                          <div class="row">
                                              <div class="col">
                                                  <h2>
                                                      4-4-4 Breathing Exercise
                                                  </h2>
                                              </div>
                                              <div class="col">
                                                  <ol>
                                                      <li>
                                                          Breathe in to a count of 4
                                                      </li>
                                                      <li>
                                                          Hold to a count of 4
                                                      </li>
                                                      <li>
                                                          Breathe out to a count of 4
                                                      </li>
                                                      <li>
                                                          Repeat 4 times
                                                      </li>
                                                  </ol>
                                              </div>
                                          </div>
                                      </div>
                                  </body>
                              </html>
                              """
                },
                new MindfulnessTip
                {
                    Id = Guid.NewGuid(),
                    Title = $"Diaphragmatic Breathing",
                    SessionRatingLevel = ((MindfulnessTipExtensions.FocusSessionRating.Fine)).ToString(),
                    Content = """
                              <html>
                                  </body>
                                      <div style="display: flex;">
                                          <div class="row">
                                              <div class="col">
                                                  <h2>
                                                      Diaphragmatic Breathing
                                                  </h2>
                                              </div>
                                              <div class="col">
                                                  <ol>
                                                      <li>
                                                          Place one hand on your chest and one hand on your diaphragm
                                                      </li>
                                                      <li>
                                                          Take a moment to notice which moves more when you breathe
                                                      </li>
                                                      <li>
                                                          Try to breathe with your diaphragm specifically if you aren't already
                                                      </li>
                                                  </ol>
                                              </div>
                                              <div class="col">
                                                  This breathing technique helps manage stress by engaging with your breathing and paying attention to your body.
                                              </div>
                                          </div>
                                      </div>
                                  </body>
                              </html>
                              """
                },
                new MindfulnessTip
                {
                    Id = Guid.NewGuid(),
                    Title = $"Placeholder Bad Tip",
                    SessionRatingLevel = ((MindfulnessTipExtensions.FocusSessionRating.Bad)).ToString(),
                    Content = """
                              <html>
                                  </body>
                                      <div style="display: flex;">
                                          <div class="row">
                                              <div class="col">
                                                  <h2>
                                                      Placeholder Bad Tip
                                                  </h2>
                                              </div>
                                          </div>
                                      </div>
                                  </body>
                              </html>
                              """
                },
            };
        }

        public async Task SeedData()
        {
            if (!_apiContext.MindfulnessTips.Any())
            {
                 var tips = getMindfulnessTips();

                await _apiContext.MindfulnessTips.AddRangeAsync(tips);

                await _apiContext.SaveChangesAsync();
            }
        }
    }
}
