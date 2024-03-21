'use client';

import { Box, Button, Group, LoadingOverlay, Textarea, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';
import { modals } from '@mantine/modals';
import { notifications } from '@mantine/notifications';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import React from 'react';
import { BookInfoDto } from '@/lib/models';
import { Api } from '@/lib/api';

export default function BookForm({ book, useModal }: { book: BookInfoDto; useModal?: boolean }) {
  const form = useForm({
    initialValues: book,
    validate: {
      name: (value) => (value ? undefined : '书名不能为空'),
    },
  });

  const queryClient = useQueryClient();
  const { mutate, isPending } = useMutation({
    mutationFn: (values: BookInfoDto) => Api.Books.Book.update(values.id, values),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['book'] });
      notifications.show({
        title: '修改成功',
        message: '',
        color: 'green',
      });
      useModal && modals.closeAll();
    },
  });

  return (
    <Box mx="auto">
      <LoadingOverlay visible={isPending} loaderProps={{ children: 'Loading...' }} />
      <form onSubmit={form.onSubmit((values) => mutate(values))}>
        <TextInput
          withAsterisk
          label="名称"
          placeholder="书名不能为空"
          {...form.getInputProps('name')}
        />

        <Textarea
          label="简介"
          placeholder="Input placeholder"
          autosize
          minRows={8}
          resize="vertical"
          {...form.getInputProps('description')}
        />

        <Group justify="flex-end" mt="md">
          <Button type="submit">保存</Button>
        </Group>
      </form>
    </Box>
  );
}
