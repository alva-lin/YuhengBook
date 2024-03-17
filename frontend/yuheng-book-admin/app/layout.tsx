import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';
import './globals.css';
import { ColorSchemeScript } from '@mantine/core';
import { MyMantineProvider, MyQueryClientProvider } from '@/lib/providers';

export const metadata = {
  title: '玉衡小说 - 后台管理',
  description: '小说站点的后台管理系统',
};

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
        <MyMantineProvider>
          <MyQueryClientProvider>{children}</MyQueryClientProvider>
        </MyMantineProvider>
      </body>
    </html>
  );
}
