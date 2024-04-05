'use client';

import { Button, Text } from '@mantine/core';
import { useQuery } from '@tanstack/react-query';
import Link from 'next/link';

import { Api } from '@/lib/api';

export default function Page({ params: { bookId } }: { params: { bookId: number } }) {
  const { data } = useQuery({
    queryKey: ['book', bookId],
    queryFn: () => Api.Books.Book.get(bookId),
  });

  return (
    <div className="w-full mt-12 px-16 flex justify-center items-center">
      <div className="flex-1 flex flex-col">
        <div className="flex justify-end">
          <Button variant="default" component={Link} href="../..">
            返回首页
          </Button>
        </div>
        {data && (
          <>
            <div className="text-4xl text-center font-bold mb-8">{data.name}</div>
            <div className="grid grid-cols-3 gap-4">
              {data.chapters.map((item) => (
                <div>
                  <Text component={Link} href={`/book/${bookId}/chapter/${item.order}`}>
                    {item.title}
                  </Text>
                </div>
              ))}
            </div>
          </>
        )}
      </div>
    </div>
  );
}
