'use client';

import Link from 'next/link';
import { redirect } from 'next/navigation';

import { Button } from '@mantine/core';
import { notifications } from '@mantine/notifications';
import { useQuery } from '@tanstack/react-query';

import Article from '@/components/Article/Article';
import { Api } from '@/lib/api';

export default function Page({
  params: { bookId, order },
}: {
  params: {
    bookId: number;
    order: number;
  };
}) {
  const { data, error } = useQuery({
    queryKey: ['book', bookId, 'chapter', order],
    queryFn: () =>
      Api.Books.Chapter.get({
        bookId,
        order,
      }),
    retry: 0,
  });

  if (error) {
    if (error.response?.data.status === 404) {
      notifications.show({
        title: '找不到指定章节',
        message: '',
        color: 'red',
      });

      redirect(`/book/${bookId}`);
    }
  }

  const prevChapter = order - 1;
  const nextChapter = order - 1 + 2;
  const tools = (
    <div className="flex gap-16 items-center justify-center">
      <Button variant="default" component={Link} href={`/book/${bookId}/chapter/${prevChapter}`}>
        上一章
      </Button>
      <Button variant="default" component={Link} href={`/book/${bookId}`}>
        目录
      </Button>
      <Button variant="default" component={Link} href={`/book/${bookId}/chapter/${nextChapter}`}>
        下一章
      </Button>
    </div>
  );

  return (
    <div className="mt-12 flex justify-center items-center">
      <div className="flex flex-col max-w-[900px] gap-8 mb-12">
        {data && (
          <>
            <div className="text-4xl text-center font-bold mb-4">{data.title}</div>
            {tools}
            <Article text={data.content} />
            {tools}
          </>
        )}
      </div>
    </div>
  );
}
