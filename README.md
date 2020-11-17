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

## MeasurementPage

På denne del af appen kan brugeren hente det seneste målte temperatur og fugtighed, udover det kan brugeren også tilføje data
som så bliver lagt op i et ThingSpeak API, hvor det biver vist i en graf.

|Flow chart **GET**|Flow Chart **POST**|
|:----------------:|:-----------------:|
|![Flowchart](./Xamarin_Projekt/Xamarin_Projekt/Images/GetMeasurement.JPG)|![Flowchart](./Xamarin_Projekt/Xamarin_Projekt/Images/PostMeasurement.JPG)|

### Filer

- MeasurementPage.xaml
- MeasurementPage.xaml.cs
- MeasurementViewModel.cs
- MeasurementService.cs

### MessagingCenter

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

### Commands

Inde under ViewModelen er der oprettet to commands, nemlig **GET** og **POST**. Brugeren har mulighed for at hente og tilføje data fra Thingspeak APIen.

```c#
public Command GetMeasurementCommand { get; }
```

```c#
public Command PostMeasurementCommand { get; set; }
```

### GetMeasurement

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

### PostMeasurement

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

## MeasurementListPage

Inde under `MeasurementListPage` kan brugeren se en liste over alle målte temperature og fugtigheder, som er blevet gemt på **ThingSpeak**.

### Filer

- MeasurementListPage.xaml
- MeasurementListPage.xaml.cs
- MeasurementListViewModel.cs
- MeasurementViewModel.cs

### Command

```c#
public Command LoadMeasurementsCommand { get; }
```

```c#
LoadMeasurementsCommand = new Command(async () => await ExecuteLoadMeasurementsCommand());
```

### ExecuteLoadMeasurementsCommand

Henter de nødvendige data til at udfylde listen. Det der bliver vist på listen for hvert måling:

- **entry_id**: Hver måling har et ID, som stiger med **1** hver gang der bliver tilføjet en måling
- **created_at**: Datoen og tidspunktet bliver også gemt, dette gør det nemmere at se hvornår dataen blev målt
- **field7**: Field7 er det *field* som bliver brugt til at gemme temperatur i en graf inde under ThingSpeak
- **field8**: Field8 er til fugtihheden

```c#
async Task ExecuteLoadMeasurementsCommand()
{
    IsBusy = true;

    try
    {
        MeasurementItems.Clear();
        var items = await GetAllMeasurements();                
        items.feeds.Reverse();
        foreach (var item in items.feeds)
        {
            item.created_at = item.created_at.Replace("T", " ");                            // Fjerner 'T' og 'Z'
            item.created_at = item.created_at.Replace("Z", " ");                            // fra datoen dataen er målt
            MeasurementItems.Add(item);
        }
    }
    catch (Exception ex)
    {

    }
    finally
    {
        IsBusy = false;
    }
}
```

`ExecuteLoadMeasurementCommand()` kører en `GetAllMeasurements()` methode. Det den gør er at den kører en anden methode, som ligger i `measurementService`.
Denne, `GetMeasurementAsync(antal)`, bliver også brugt i **MeasurementPage**, men da bliver der ikke hentet *1000* målinger, men kun *1*. Dette kunne også
have blevet lavet dynamisk, så brugeren vælger hvor mange målinger der skal hentes.

```c#
public async Task<Measurements> GetAllMeasurements()
{
    return await _measurementService.GetMeasurementAsync(1000);                             // Henter de seneste 1000 målinger fra APIen
}
```

## Screenshots

|Measurement Page|Measurement Success|Measurement Load|Measurement List Page|
|:--:|:--:|:--:|:--:|
|![Measurement Page](./Xamarin_Projekt/Xamarin_Projekt/Images/MeasurementPage.png)|![Measurement Success](./Xamarin_Projekt/Xamarin_Projekt/Images/MeasurementSuccess.png)|![Measurement Load](./Xamarin_Projekt/Xamarin_Projekt/Images/MeasurementLoad.png)|![Measurement List Page](./Xamarin_Projekt/Xamarin_Projekt/Images/MeasurementListPage.png)

|ThingSpeak|
|:--:|
![ThingSpeak måling](./Xamarin_Projekt/Xamarin_Projekt/Images/ThingSpeak.png)|