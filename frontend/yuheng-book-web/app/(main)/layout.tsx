'use client';

import { Suspense } from 'react';

import { AppShell, rem } from '@mantine/core';
import { useHeadroom } from '@mantine/hooks';

import { Footer } from '@/components/Layouts/footer';
import { Header } from '@/components/Layouts/header';
import Loading from '@/components/Loading';

export default function Layout({ children }: { children: any }) {
  const pinned = useHeadroom({ fixedAt: 120 });

  return (
    <AppShell
      header={{
        height: 60,
        collapsed: !pinned,
        offset: false,
      }}
    >
      <AppShell.Header>
        <Header />
      </AppShell.Header>

      <AppShell.Main
        pt={`calc(${rem(60)} + var(--mantine-spacing-md))`}
        className="flex flex-col justify-between"
      >
        <AppShell.Section
          className="flex-1"
          style={{
            position: 'relative',
            zIndex: 1,
            backgroundColor: 'var(--mantine-color-body)',
            boxShadow: 'var(--mantine-shadow-md)',
            // minHeight: 'calc(100vh - 400px)',
            paddingLeft: 'var(--mantine-spacing-md)',
            paddingRight: 'var(--mantine-spacing-md)',
          }}
        >
          <Suspense fallback={<Loading />}>{children}</Suspense>
        </AppShell.Section>
        <Footer />
      </AppShell.Main>
    </AppShell>
  );
}
