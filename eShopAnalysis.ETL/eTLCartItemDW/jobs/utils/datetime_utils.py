import pandas as pd
def generate_date_df(start=pd.Timestamp('2021-01-01 00:00:00'), end=pd.Timestamp('2021-12-31 00:00:00')):
    df = pd.DataFrame(pd.date_range(start, end, freq='1min'), columns=['Date'])
    # df = pd.DataFrame(pd.date_range(start, end), columns=['Date'])
    df['DateKey'] = df['Date'].dt.strftime('%Y%m%d%H%M')
    df['Year'] = df['Date'].dt.year
    df['Month'] = df['Date'].dt.month
    df['Quarter'] = df['Date'].dt.quarter
    df['Day'] = df['Date'].dt.day
    df['Weekday'] = df['Date'].dt.weekday
    df['WeekdayName'] = df['Date'].dt.day_name()
    df.to_csv('resources/date.csv', index=False)
    return df
