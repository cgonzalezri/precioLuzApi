using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using precioLuzApi.DTOs;

namespace precioLuzApi.Services
{
    public class GetDataService
    {
        public IConfiguration Configuration { get; }

        public GetDataService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public async Task<List<PriceDataDTO>> GetDataAsync()
        {
            List<PriceDataDTO> data = new List<PriceDataDTO>();

            try
            {
                HttpClient client = new HttpClient();
            
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Configuration["ReeToken"]);

                HttpResponseMessage response = await client.GetAsync("https://api.esios.ree.es/archives/70/download_json");
                
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                if(!string.IsNullOrEmpty(responseBody))
                {
                    JsonDocument jsonDocument = JsonDocument.Parse(responseBody);

                    data = JsonSerializer.Deserialize<List<PriceDataDTO>>(jsonDocument.RootElement.GetProperty("PVPC").ToString());

                    foreach(var element in data)
                    {
                        Single newPCB = 0;

                        Single.TryParse(element.PCB, out newPCB);

                        if (newPCB > 0)
                        {
                            switch(Thread.CurrentThread.CurrentCulture.Name)
                            {
                                case "en-US":
                                    element.PCB = (newPCB / 100).ToString("0,000");
                                    break;

                                default:
                                    element.PCB = (newPCB / 1000).ToString("0.000");
                                    break;
                            }
                        }

                        element.Hora = element.Hora.Substring(0,2);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return data;
        }
    }
}