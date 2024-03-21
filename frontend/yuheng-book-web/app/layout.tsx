import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';
import './globals.css';
import { ColorSchemeScript, createTheme, MantineProvider } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import { MyQueryClientProvider } from '@/lib/providers';

export const metadata = {
  title: '玉衡小说',
  description: '免费的无弹窗小说站点',
};
const siteTheme = createTheme({
  primaryColor: 'dark',
});

export default function RootLayout({ children }: { children: any }) {
  return (
    <html lang="en">
    <head>
      <ColorSchemeScript />
      <link rel="shortcut icon" href="/favicon.svg" />
      <meta
        name="viewport"
        content="minimum-scale=1, initial-scale=1, width=device-width, user-scalable=no"
      />
    </head>
    <body>
    <MantineProvider theme={siteTheme}>
      <Notifications autoClose={5000} />
      <MyQueryClientProvider>
        {children}
      </MyQueryClientProvider>
    </MantineProvider>
    </body>
    </html>
  );
}
