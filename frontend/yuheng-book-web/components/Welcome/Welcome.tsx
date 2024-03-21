import { Anchor, Text } from '@mantine/core';

export function Welcome() {
  return (
    <>
      <div className="mt-32 text-center font-black tracking-widest text-6xl flex justify-center gap-2">
        <span className="i-fluent-book-24-regular" role="img" aria-hidden="true" />
        <span>玉衡小说网</span>
      </div>
      <Text c="dimmed" ta="center" size="lg" maw={580} mx="auto" mt="xl">
        This starter Next.js project includes a minimal setup for server side rendering, if you want
        to learn more on Mantine + Next.js integration follow{' '}
        <Anchor href="https://mantine.dev/guides/next/" size="lg">
          this guide
        </Anchor>
        . To get started edit page.tsx file.
      </Text>
    </>
  );
}
