'use client';

import { ActionIcon, Button, Group, Text } from '@mantine/core';
import { modals } from '@mantine/modals';
import { notifications } from '@mantine/notifications';
import { Welcome } from '@/components/Welcome/Welcome';
import { ColorSchemeToggle } from '@/components/ColorSchemeToggle/ColorSchemeToggle';

export default function HomePage() {
  return (
    <>
      <div className="flex justify-center content-center items-center h-32 bg-gradient-to-r from-pink-400 to-yellow-500  gap-2">
        <ActionIcon aria-label="bank icon" size="xl" variant="outline" color="white">
          <span
            className="i-fluent-building-bank-24-regular text-3xl text-white"
            role="img"
            aria-hidden="true"
          />
        </ActionIcon>
        <h1 className="font-bold text-4xl text-white ">Hello World</h1>
      </div>
      <Group mt="md">
        <Button
          onClick={() => {
            notifications.show({
              title: 'Hello, World!',
              message: 'This is a notification from Mantine',
            });
          }}
        >
          Show notification
        </Button>
        <Button
          onClick={() => {
            modals.openConfirmModal({
              title: 'Hello, World!',
              children: <Text size="sm">This is a modal from Mantine</Text>,
              labels: {
                cancel: 'Close',
                confirm: 'Confirm',
              },
              onConfirm: () => {
                notifications.show({
                  title: 'Confirmed',
                  message: 'You confirmed modal',
                });
              },
              onCancel: () => {
                notifications.show({
                  title: 'Canceled',
                  message: 'You canceled modal',
                });
              },
            });
          }}
        >
          Show modal
        </Button>
      </Group>
      <Welcome />
      <ColorSchemeToggle />
    </>
  );
}
