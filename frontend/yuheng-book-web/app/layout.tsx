import { ColorSchemeScript, MantineProvider, createTheme } from '@mantine/core';
import '@mantine/core/styles.css';
import { Notifications } from '@mantine/notifications';
import '@mantine/notifications/styles.css';

import { MyQueryClientProvider } from '@/lib/providers';

import './globals.css';

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
          <MyQueryClientProvider>{children}</MyQueryClientProvider>
        </MantineProvider>
      </body>
    </html>
  );
}
