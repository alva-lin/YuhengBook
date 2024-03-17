import {
  Avatar,
  Burger,
  Button,
  Code,
  Flex,
  Group,
  Menu,
  rem,
  useMantineColorScheme,
} from '@mantine/core';
import React from 'react';

export type HeaderProps = {
  opened: boolean;
  toggle: () => void;
};

export function Header({ opened, toggle }: HeaderProps) {
  const { colorScheme, toggleColorScheme } = useMantineColorScheme({
    keepTransitions: true,
  });

  return (
    <Flex justify="space-between" align="center" wrap="nowrap" h={rem(60)} px="md">
      <Group>
        <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
        <Button
          variant="subtle"
          size="md"
          px="sm"
          leftSection={
            <span className="i-fluent-book-24-regular text-4xl" role="img" aria-hidden="true" />
          }
        >
          <span className="text-2xl">玉衡小说</span>
        </Button>
        <Code fw={700}>v0.0.1</Code>
      </Group>
      <Group align="flex-end">
        <Menu
          position="bottom-end"
          offset={5}
          shadow="md"
          transitionProps={{
            transition: 'pop-top-right',
            duration: 150,
          }}
        >
          <Menu.Target>
            <Avatar component="button" radius="sm" alt="no image here">
              {'Alva'.toUpperCase().substring(0, 2)}
            </Avatar>
          </Menu.Target>
          <Menu.Dropdown>
            <Menu.Item
              leftSection={
                colorScheme === 'dark' ? (
                  <span
                    className="i-fluent-weather-moon-24-regular text-lg"
                    role="img"
                    aria-hidden="true"
                  />
                ) : (
                  <span
                    className="i-fluent-weather-sunny-24-regular text-lg"
                    role="img"
                    aria-hidden="true"
                  />
                )
              }
              onClick={() => {
                toggleColorScheme();
              }}
            >
              {colorScheme === 'dark' ? 'Dark Mode' : 'Light Mode'}
            </Menu.Item>
            <Menu.Divider />
            <Menu.Item
              leftSection={
                <span
                  className="i-fluent-person-24-regular text-lg"
                  role="img"
                  aria-hidden="true"
                />
              }
              onClick={() => {
                console.log('Profile clicked');
              }}
            >
              Profile
            </Menu.Item>
            <Menu.Item
              leftSection={
                <span
                  className="i-fluent-settings-24-regular text-lg"
                  role="img"
                  aria-hidden="true"
                />
              }
              onClick={() => {
                console.log('Settings clicked');
              }}
            >
              Settings
            </Menu.Item>

            <Menu.Divider />
            <Menu.Item
              color="red"
              leftSection={
                <span
                  className="i-fluent-arrow-turn-down-right-20-regular text-lg"
                  role="img"
                  aria-hidden="true"
                />
              }
              onClick={() => {
                console.log('Logout clicked');
              }}
            >
              Logout
            </Menu.Item>
          </Menu.Dropdown>
        </Menu>
      </Group>
    </Flex>
  );
}
