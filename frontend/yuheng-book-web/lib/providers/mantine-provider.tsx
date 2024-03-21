import { createTheme, MantineProvider } from '@mantine/core';
import { Notifications } from '@mantine/notifications';

const siteTheme = createTheme({
  primaryColor: 'dark',
});

export function MyMantineProvider({ children }: { children: React.ReactNode }) {
  return (
    <MantineProvider theme={siteTheme}>
      <Notifications autoClose={5000} />
      {children}
    </MantineProvider>
  );
}
