# App programmering 3 projekt - Kenneth Jessen

## Krav til projektet

- Projektet går ud på at lave en app, der kan hente temperatur- og fugtighedsmålinger fra Thingspeak cloudservicen.

- Der skal præsenteres aktuel temperatur og humidity fra den seneste måling. Desuden skal man kunne vise en række målinger og gerne inddelt i grupper svarende enten timer eller dage.

- Som options kan man lave en mulighed for at kunne vælge forskellige målesessions, f.eks. svarende til forskellige rum i huset.

- Man kan også ønske sig en alarm, der adviserer om at temperaturen er kommet udenfor en given grænse.

- Og endelig vil det også være flot med et koordinatsystem, som præsenterer målingerne over en given tid.

- Projektet afleveres i Github og præsenteres for klassen.

## Indledning

Projektet er lavet med et flyout shell template, det vil sige at menuen og nogle features allerede er lavet.
Projektet er også lavet med MVVM (Model View ViewModel), som så gør det nemmere og mere overskueligt at læse koden.
Det meste af koden er skrevet asynchront, så brugeren kan bruge appen imens at der bliver udført features.

### Features

- Appen kan hente det seneste målte temperatur og fugtighed
- Brugeren kan tilføje temperatur og fugtighed
- Der er oprettet en exceptionhandling, så brugeren kun kan tilføje data ca. hvert 15 sek
- Der er mulighed for at se alle målte temperature og fugtigheder

### Andre features som er implementeret

- MVVM
- Behavior
- TinyIoC
- Polly
- Shell

### Flowchart

![Flowchart](./Xamarin_Projekt/Xamarin_Projekt/Images/AppFlowchart.JPG)

## Kode

### MeasurementPage

#### Filer

- MeasurementPage.xaml
- MeasurementPage.xaml.cs
- MeasurementViewModel.cs
- MeasurementService.cs

#### MessagingCenter

Der er oprette to messages, en til hvis brugeren ikke har indtastet et valid tal. Den anden er når brugeren har tilføjet data til APIen og det er gået igennem.

```c#
// Alert som vises hvis brugeren prøvet at sende noget til API'en som ikke er et tal
MessagingCenter.Subscribe<MeasurementViewModel>(this, "InvalidEntry", (sender) =>
{
    DisplayAlert("Error", "The entries must be a number!", "OK");
});
```

```c#
// Alert som vises når dataen er blevet sendt til API'en
MessagingCenter.Subscribe<MeasurementViewModel>(this, "ValidEntry", (sender) =>
{
    DisplayAlert("Success", "The temperatur and humidity has been saved and send to the Thingspeak API!", "OK");
});
```

#### Commands

Inde under ViewModelen er der oprettet to commands, nemlig **GET** og **POST**. Brugere har mulighed for at hente og tilføje data fra Thingspeak APIen.

```c#
public Command GetMeasurementCommand { get; }
```

```c#
public Command PostMeasurementCommand { get; set; }
```

#### GetMeasurement

Kommandoen bliver sat til at skulle køre en methode.

```c#
GetMeasurementCommand = new Command(async () => await GetMeasurement(1));
```

Methoden som bliver kørt tager imod en int, det den int gør er at den henter antal temperature og fugtigheder målinger.
Methoden i sig selv henter dataen fra den service som er oprettet, derefter sætter den de rigtig værdier ind, så det kan vises til brugeren.

```c#
async Task GetMeasurement(int amount)
{
    var items = await _measurementService.GetMeasurementAsync(amount);
    foreach (var item in items.feeds)
    {
        Temperatur = item.field7.ToString();
        Humidity = item.field8.ToString();
    }

    RefreshCanExecutes();
}
```

**GetMeasurementAsync** opretter den API URL som skal hente dataen fra ThingSpeak API.
Ved hjælp af UriBuilder kan der blive oprettet en API URL.

```c#
public async Task<Measurements> GetMeasurementAsync(int amount)
{
    UriBuilder builder = new UriBuilder(ApiConstants.ApiURL)
    {
        // https://api.thingspeak.com/channels/1217134/feeds.json?api_key=ZH6EGHKLH20U4K54&results=1               // result=1, betyder at den henter det seneste målte data

        Path = $"channels/{ApiConstants.ApiID}/feeds.json",
        Query = $"api_key={ApiConstants.ApiKeyRead}&results={amount}"
    };

    return await _genericRepository.GetAsync<Measurements>(builder.ToString());
}
```
#### Hentning af data fra API - Flowchart

![Flowchart](./Xamarin_Projekt/Xamarin_Projekt/Images/GetMeasurement.JPG)

#### PostMeasurement

Inden at methoden bliver kørt som rent faktisk tilføjer dataen ud til APIen, er der lavet en validering til om brugeren har indtastet et nummer eller ej.
Hvis brugeren ikke har bliver der sendt en Alert ud med at der er indtastet et ugyldig tegn.

```c#
PostMeasurementCommand = new Command(
execute: async () =>
{
    bool validationCheck = double.TryParse(_temperatur, out _) && double.TryParse(_humidity, out _);

    if (validationCheck == false)
    {
        MessagingCenter.Send(this, "InvalidEntry");
    }
    else
    {
        IsBusy = true;
        IsValid = false;
        await PostMeasurement();
        MessagingCenter.Send(this, "ValidEntry");
        IsBusy = false;
        IsValid = true;
    }
});
```

**POST** methoden minder lidt om **GET**, men istedet for at initializere dataen, så bliver det hentet fra temperatur og fugtigheds entry.

```c#
async Task PostMeasurement()
{
    MeasurementItem measurements = new MeasurementItem()
    {
        field7 = Convert.ToDouble(_temperatur),                                     // Temperatur
        field8 = Convert.ToDouble(_humidity)                                        // Fugtighed
    };

    await _measurementService.PostMeasurementAsync(measurements);

    RefreshCanExecutes();
}
```

Ligesom henting af data, skal der også laves en API URL som tilføjer data.

```c#
public async Task<bool> PostMeasurementAsync(MeasurementItem measurements)
{
    UriBuilder builder = new UriBuilder(ApiConstants.ApiURL)
    {
        // https://api.thingspeak.com/update?api_key=198AI1XVNPPEIPEE&field7=22&field8=30

        Path = $"update",
        Query = $"api_key={ApiConstants.ApiKeyWrite}&field7={measurements.field7}&field8={measurements.field8}"
    };

    await _genericRepository.PostAsync<MeasurementItem>(builder.ToString(), measurements);
    return true;
}
```

#### Tilføjelse af data til API - Flowchart

![Flowchart](./Xamarin_Projekt/Xamarin_Projekt/Images/PostMeasurement.JPG)

### MeasurementListPage