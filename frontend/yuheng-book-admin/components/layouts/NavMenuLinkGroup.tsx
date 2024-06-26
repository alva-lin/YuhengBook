import { Box, Collapse, Group, rem, Text, ThemeIcon, UnstyledButton } from '@mantine/core';
import { IconChevronRight } from '@tabler/icons-react';
import Link from 'next/link';
import React, { useState } from 'react';
import classes from './NavMenuLinkGroup.module.css';

interface LinksGroupProps {
  icon: React.FC<any>;
  label: string;
  initiallyOpened?: boolean;
  links?: { label: string; href: string }[];

  href?: string;
}

export function LinksGroup({ icon: Icon, label, initiallyOpened, links, href }: LinksGroupProps) {
  const hasLinks = Array.isArray(links);
  const [opened, setOpened] = useState(initiallyOpened || false);
  const items = (hasLinks ? links : []).map((link) => (
    <Text component={Link} className={classes.link} href={link.href} key={link.href + link.label}>
      {link.label}
    </Text>
  ));

  if (href && !hasLinks) {
    return (
      <>
        <UnstyledButton component={Link} href={href} className={classes.control}>
          <Group justify="space-between" gap={0}>
            <Box
              style={{
                display: 'flex',
                alignItems: 'center',
              }}
            >
              <ThemeIcon variant="default" size="md">
                <Icon
                  style={{
                    width: rem(18),
                    height: rem(18),
                  }}
                />
              </ThemeIcon>
              <Box ml="md">{label}</Box>
            </Box>
          </Group>
        </UnstyledButton>
      </>
    );
  }

  return (
    <>
      <UnstyledButton onClick={() => setOpened((o) => !o)} className={classes.control}>
        <Group justify="space-between" gap={0}>
          <Box
            style={{
              display: 'flex',
              alignItems: 'center',
            }}
          >
            <ThemeIcon variant="default" size="md">
              <Icon
                style={{
                  width: rem(18),
                  height: rem(18),
                }}
              />
            </ThemeIcon>
            <Box ml="md">{label} </Box>
          </Box>
          {hasLinks && (
            <IconChevronRight
              className={classes.chevron}
              stroke={1.5}
              style={{
                width: rem(16),
                height: rem(16),
                transform: opened ? 'rotate(-90deg)' : 'none',
              }}
            />
          )}
        </Group>
      </UnstyledButton>
      {hasLinks ? <Collapse in={opened}>{items}</Collapse> : null}
    </>
  );
}
