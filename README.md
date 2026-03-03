Note to the code examiner: Thank you for providing this opportunity. I have implemented the IntradayReportGenerator application as a window service that runs in the background and generates intraday reports based on the data retrieved from the PowerService(dll).
I have tried to minimise comment where possible as i believe the code should be self documenting to some extent. However, I have added comments where I felt it was necessary to explain the logic or the purpose of a particular section of code.

This application is built with clean and simple architecture in mind leading to the following design decisions.

1. IntradayReportGenerator is responsible for orchestrating the integration of different services getting called by it when it runs as a background service.
2. Aggregation of volumes alongside mapping of periods (int to Time Component) happens in TradeAggregator class.
3. To facilitate the second point PeriodToDateTimeHelper(mapper) class is created which has the one to one mapping of period (int) to time Component.
4. The Volume retrieved from PowerService class is rounded off to two decimal places.
5. CsvGenerator class handles the logic to export the data to a csv file after retrieving aggregated data.
6. By default the time delay is considered in minutes. The timeinterval for each cycle is set at 1minute and extract location is set at C:\\Temp\\Extracts in the config file.
7. Appropriate logging with exception handlers are placed for diagnositc purposes and to ensure the application runs smoothly without crashing in case of any unexpected errors.
8. No github branching strategy is implemented as this is a single developer project for the purpose of the assessment.
9. Tests are added where necessary to ensure the quality of the application.
10. Retry logic has been added for PowerService dll call given the issues that was reported to ensure at few retries are attempted before logging error.
